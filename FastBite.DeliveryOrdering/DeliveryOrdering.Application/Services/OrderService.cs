
using AutoMapper;
using DeliveryOrdering.Application.DTOs;
using DeliveryOrdering.Application.Exceptions;
using DeliveryOrdering.Application.Interfaces;
using DeliveryOrdering.Domain.Entities;
using DeliveryOrdering.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryOrdering.Application.Services
{
    public class OrderService : IOrder
    {
        private readonly IMenuCatalogService _catalogService;
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderService> _logger;

        private const decimal DescontoCombo = 0.10m;

        private const decimal TaxaEntregaPadrao = 2.50m;
        private const int MaxQuantidadePorLinha = 50;
        private const int MaxUnidadesPorPedido = 200;

        public OrderService(
            IMenuCatalogService catalogService,
            IOrderRepository orderRepository,
            IMapper mapper,
            ILogger<OrderService> logger)
        {
            _catalogService = catalogService;
            _orderRepository = orderRepository;
            _mapper = mapper;
            _logger = logger;
        }

       public async Task<AgrupamentoCarrinhoResponseDto> AgruparItemsAsync(AgruparItemsRequestDto dto)
        {
            var itens = NormalizarItens(dto?.Items);
            var agrupamento = await AgruparAsync(itens);
            var taxaEntrega = ObterTaxaEntrega(dto!.TaxaEntrega);
            return MapearPreview(agrupamento, taxaEntrega);
        }

        public async Task<OrderHistoryResponseDto> CriarPedidoAsync(CreateOrderRequestDto dto, string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new PedidoInvalidoException("Utilizador não autenticado.");

            ValidarEntregaEPagamento(dto);

            var itens = NormalizarItens(dto.Items);
            var agrupamento = await AgruparAsync(itens);            // re-valida catálogo e stock AGORA

            if (agrupamento.Indisponiveis.Count > 0)
                throw new StockInsuficienteException(agrupamento.Indisponiveis);

            if (dto.SubtotalEsperado.HasValue && dto.SubtotalEsperado.Value != agrupamento.Subtotal)
                throw new PrecoAlteradoException(dto.SubtotalEsperado.Value, agrupamento.Subtotal);

            var taxaEntrega = ObterTaxaEntrega(dto.Pagamento!.TaxaEntrega);
            var pedido = ConstruirOrder(agrupamento, dto, userId, taxaEntrega);

            await _orderRepository.AdicionarAsync(pedido);
            await _orderRepository.SaveChangesAsync();

            // payload = carrinho normalizado
            var payload = itens
                .Select(i => new ItemVendidoDto { ItemId = i.ProductId, Quantidade = i.Quantity })
                .ToList();

            try
            {
                await _catalogService.DescontarStockAsync(payload);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Pedido {OrderId} gravado mas falhou o desconto de stock. A cancelar.", pedido.Id);
                await TentarCancelarAsync(pedido);
                throw new DescontoStockFalhouException(pedido.Id, ex);
            }

            return _mapper.Map<OrderHistoryResponseDto>(pedido);
        }

        public async Task<IEnumerable<OrderHistoryResponseDto>> GetUserOrderHistoryAsync(string userId)
        {
            var orders = await _orderRepository.GetOrdersByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<OrderHistoryResponseDto>>(orders);
        }

        private enum CategoriaCombo { Prato, Acompanhamento, Bebida, Outro }

        private sealed record Unidade(int ProductId, MenuResponseDto Menu, CategoriaCombo Categoria);

        private sealed record ComboAgrupado(
            Unidade Prato, Unidade Acompanhamento, Unidade Bebida,
            int Quantity, decimal PrecoOriginal, decimal PrecoComDesconto);

        private sealed record AvulsoAgrupado(int ProductId, MenuResponseDto Menu, int Quantity);

        private sealed class Agrupamento
        {
            public List<ComboAgrupado> Combos { get; } = new();
            public List<AvulsoAgrupado> Avulsos { get; } = new();
            public List<CreateOrderItemDto> Itens { get; init; } = new();
            public List<ItemIndisponivelDto> Indisponiveis { get; } = new();
            public decimal Subtotal { get; set; }
            public decimal TotalDescontos { get; set; }
        }

        /// <summary>Valida quantidades, soma linhas duplicadas do mesmo ProductId e impõe limites.</summary>
        private static List<CreateOrderItemDto> NormalizarItens(IEnumerable<CreateOrderItemDto>? items)
        {
            if (items == null)
                throw new PedidoInvalidoException("O carrinho está vazio.");

            var lista = items.ToList();
            if (lista.Count == 0)
                throw new PedidoInvalidoException("O carrinho está vazio.");

            foreach (var i in lista)
            {
                if (i == null) throw new PedidoInvalidoException("Item inválido.");
                if (i.ProductId <= 0) throw new PedidoInvalidoException($"ProductId inválido: {i.ProductId}.");
                if (i.Quantity <= 0) throw new PedidoInvalidoException($"Quantidade inválida para o produto {i.ProductId}.");
            }

            var normalizados = lista
                .GroupBy(i => i.ProductId)
                .Select(g => new CreateOrderItemDto { ProductId = g.Key, Quantity = g.Sum(x => x.Quantity) })
                .OrderBy(i => i.ProductId)
                .ToList();

            if (normalizados.Any(i => i.Quantity > MaxQuantidadePorLinha))
                throw new PedidoInvalidoException($"Quantidade máxima por produto é {MaxQuantidadePorLinha}.");

            if (normalizados.Sum(i => i.Quantity) > MaxUnidadesPorPedido)
                throw new PedidoInvalidoException($"Máximo de {MaxUnidadesPorPedido} unidades por pedido.");

            return normalizados;
        }

        /// <summary>
        /// Núcleo do algoritmo: busca cada menu UMA vez, verifica disponibilidade com a quantidade
        /// agregada, expande em unidades, forma combos (prato+acomp+bebida, -10%) e agrupa o resto.
        /// Puro no que toca a efeitos secundários: não grava nem desconta nada.
        /// </summary>
        private async Task<Agrupamento> AgruparAsync(List<CreateOrderItemDto> itens)
        {
            var resultado = new Agrupamento { Itens = itens };
            var unidades = new List<Unidade>(capacity: itens.Sum(i => i.Quantity));

            foreach (var item in itens)
            {
                var menu = await _catalogService.ObterMenuPorIdAsync(item.ProductId)
                           ?? throw new ProdutoNaoEncontradoException(item.ProductId);

                // Validação de campos do catálogo no boundary
                ValidarProdutoCatalogo(item.ProductId, menu);

                bool disponivel = await _catalogService.VerificarDisponibilidadeAsync(item.ProductId, item.Quantity);
                if (!disponivel)
                {
                    resultado.Indisponiveis.Add(new ItemIndisponivelDto
                    {
                        ProductId = item.ProductId,
                        Nome = menu.Nome,
                        QuantidadePedida = item.Quantity
                    });
                }

                var categoria = ClassificarCategoria(menu.Categoria);
                for (int i = 0; i < item.Quantity; i++)
                    unidades.Add(new Unidade(item.ProductId, menu, categoria));
            }

            // Ordenação descendente por preço: os combos são formados com os itens mais caros,
            // o que maximiza o desconto para o cliente.
            var pratos = Fila(unidades, CategoriaCombo.Prato);
            var acomps = Fila(unidades, CategoriaCombo.Acompanhamento);
            var bebidas = Fila(unidades, CategoriaCombo.Bebida);
            var outros = unidades.Where(u => u.Categoria == CategoriaCombo.Outro).ToList();

            int nCombos = Math.Min(pratos.Count, Math.Min(acomps.Count, bebidas.Count));

            var combos = new List<(Unidade P, Unidade A, Unidade B)>(nCombos);
            for (int i = 0; i < nCombos; i++)
                combos.Add((pratos[i], acomps[i], bebidas[i]));

            // Combos idênticos -> uma linha com Quantity > 1
            foreach (var g in combos.GroupBy(c => (c.P.ProductId, c.A.ProductId, c.B.ProductId)))
            {
                var (p, a, b) = g.First();
                decimal precoOriginal = p.Menu.PrecoBase + a.Menu.PrecoBase + b.Menu.PrecoBase;
                decimal precoComDesconto = ArredondarMoeda(precoOriginal * (1 - DescontoCombo));
                int qtd = g.Count();

                resultado.Combos.Add(new ComboAgrupado(p, a, b, qtd, precoOriginal, precoComDesconto));
                resultado.Subtotal += precoComDesconto * qtd;
                resultado.TotalDescontos += (precoOriginal - precoComDesconto) * qtd;
            }

            // Restantes (incluindo categorias fora do combo — nunca são descartadas)
            var restantes = pratos.Skip(nCombos)
                .Concat(acomps.Skip(nCombos))
                .Concat(bebidas.Skip(nCombos))
                .Concat(outros);

            foreach (var g in restantes.GroupBy(u => u.ProductId))
            {
                var menu = g.First().Menu;
                int qtd = g.Count();
                resultado.Avulsos.Add(new AvulsoAgrupado(g.Key, menu, qtd));
                resultado.Subtotal += ArredondarMoeda(menu.PrecoBase) * qtd;
            }

            resultado.Subtotal = ArredondarMoeda(resultado.Subtotal);
            resultado.TotalDescontos = ArredondarMoeda(resultado.TotalDescontos);
            return resultado;
        }

        private static List<Unidade> Fila(IEnumerable<Unidade> unidades, CategoriaCombo cat) =>
            unidades.Where(u => u.Categoria == cat)
                    .OrderByDescending(u => u.Menu.PrecoBase)
                    .ThenBy(u => u.ProductId)          // determinístico em caso de empate
                    .ToList();

        /// <summary>Matching tolerante: trim, case-insensitive, sem acentos, aceita plural.</summary>
        private static CategoriaCombo ClassificarCategoria(string? categoria)
        {
            var c = RemoverAcentos((categoria ?? string.Empty).Trim()).ToLowerInvariant();
            return c switch
            {
                "prato" or "pratos" => CategoriaCombo.Prato,
                "acompanhamento" or "acompanhamentos" => CategoriaCombo.Acompanhamento,
                "bebida" or "bebidas" => CategoriaCombo.Bebida,
                _ => CategoriaCombo.Outro
            };
        }

        private static void ValidarProdutoCatalogo(int productId, MenuResponseDto menu)
        {
            if (string.IsNullOrWhiteSpace(menu.Nome))
                throw new PedidoInvalidoException($"Produto {productId} com nome vazio no catálogo.");
            if (menu.PrecoBase < 0m)
                throw new PedidoInvalidoException($"Produto {productId} com preço negativo no catálogo.");
        }

        private static string RemoverAcentos(string s)
        {
            var d = s.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder(d.Length);
            foreach (var ch in d)
                if (CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
                    sb.Append(ch);
            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        private static decimal ArredondarMoeda(decimal valor) =>
            Math.Round(valor, 2, MidpointRounding.AwayFromZero);

        private static decimal ObterTaxaEntrega(decimal? pedida)
        {
            // TODO(segurança): calcular no servidor (por código postal / zona). Ver riscos no relatório.
            var taxa = pedida ?? TaxaEntregaPadrao;
            if (taxa < 0) throw new PedidoInvalidoException("Taxa de entrega inválida.");
            return ArredondarMoeda(taxa);
        }

        private static void ValidarEntregaEPagamento(CreateOrderRequestDto? dto)
        {
            if (dto == null) throw new PedidoInvalidoException("Pedido vazio.");

            var e = dto.Entrega ?? throw new PedidoInvalidoException("Dados de entrega em falta.");
            if (string.IsNullOrWhiteSpace(e.NomeCompleto)) throw new PedidoInvalidoException("Nome completo obrigatório.");
            if (string.IsNullOrWhiteSpace(e.ContactoTelefonico)) throw new PedidoInvalidoException("Contacto telefónico obrigatório.");
            if (string.IsNullOrWhiteSpace(e.Morada)) throw new PedidoInvalidoException("Morada obrigatória.");
            if (string.IsNullOrWhiteSpace(e.CodigoPostal)) throw new PedidoInvalidoException("Código postal obrigatório.");
            if (string.IsNullOrWhiteSpace(e.Cidade)) throw new PedidoInvalidoException("Cidade obrigatória.");

            var p = dto.Pagamento ?? throw new PedidoInvalidoException("Dados de pagamento em falta.");
            if (string.IsNullOrWhiteSpace(p.MetodoPagamento)) throw new PedidoInvalidoException("Método de pagamento obrigatório.");
            if (p.TaxaEntrega < 0) throw new PedidoInvalidoException("Taxa de entrega inválida.");
        }

        private static AgrupamentoCarrinhoResponseDto MapearPreview(Agrupamento a, decimal taxaEntrega)
        {
            var dto = new AgrupamentoCarrinhoResponseDto
            {
                Items = a.Itens,
                Subtotal = a.Subtotal,
                TotalDescontos = a.TotalDescontos,
                TaxaEntrega = taxaEntrega,
                Total = ArredondarMoeda(a.Subtotal + taxaEntrega),
                Indisponiveis = a.Indisponiveis
            };

            foreach (var c in a.Combos)
            {
                dto.Combos.Add(new ComboPreviewDto
                {
                    PratoId = c.Prato.ProductId,
                    PratoNome = c.Prato.Menu.Nome,
                    AcompanhamentoId = c.Acompanhamento.ProductId,
                    AcompanhamentoNome = c.Acompanhamento.Menu.Nome,
                    BebidaId = c.Bebida.ProductId,
                    BebidaNome = c.Bebida.Menu.Nome,
                    Descricao = DescricaoCombo(c),
                    Quantity = c.Quantity,
                    PrecoOriginalUnitario = ArredondarMoeda(c.PrecoOriginal),
                    UnitPrice = c.PrecoComDesconto,
                    DescontoUnitario = ArredondarMoeda(c.PrecoOriginal - c.PrecoComDesconto),
                    TotalLinha = ArredondarMoeda(c.PrecoComDesconto * c.Quantity)
                });
            }

            foreach (var v in a.Avulsos)
            {
                dto.Avulsos.Add(new ItemAvulsoPreviewDto
                {
                    ProductId = v.ProductId,
                    Nome = v.Menu.Nome,
                    Categoria = v.Menu.Categoria ?? string.Empty,
                    Quantity = v.Quantity,
                    UnitPrice = ArredondarMoeda(v.Menu.PrecoBase),
                    TotalLinha = ArredondarMoeda(v.Menu.PrecoBase * v.Quantity)
                });
            }

            return dto;
        }

        private static Order ConstruirOrder(Agrupamento a, CreateOrderRequestDto dto, string userId, decimal taxaEntrega)
        {
            var pedido = new Order
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pendente,
                Items = new List<OrderItem>(),
                NomeCompleto = dto.Entrega!.NomeCompleto.Trim(),
                ContactoTelefonico = dto.Entrega.ContactoTelefonico.Trim(),
                Morada = dto.Entrega.Morada.Trim(),
                CodigoPostal = dto.Entrega.CodigoPostal.Trim(),
                Cidade = dto.Entrega.Cidade.Trim(),
                MetodoPagamento = dto.Pagamento!.MetodoPagamento.Trim(),
                TaxaEntrega = taxaEntrega,
                Subtotal = a.Subtotal,
                TotalAmount = ArredondarMoeda(a.Subtotal + taxaEntrega)
            };

            foreach (var c in a.Combos)
            {
                pedido.Items.Add(new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = pedido.Id,
                    Type = OrderItemType.Combo,
                    ProductId = c.Prato.ProductId,
                    DescricaoItem = DescricaoCombo(c),
                    AcompanhamentoId = c.Acompanhamento.ProductId,
                    AcompanhamentoNome = c.Acompanhamento.Menu.Nome,
                    BebidaId = c.Bebida.ProductId,
                    BebidaNome = c.Bebida.Menu.Nome,
                    Quantity = c.Quantity,
                    UnitPrice = c.PrecoComDesconto
                });
            }

            foreach (var v in a.Avulsos)
            {
                pedido.Items.Add(new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = pedido.Id,
                    Type = OrderItemType.Avulso,
                    ProductId = v.ProductId,
                    DescricaoItem = v.Menu.Nome,
                    Quantity = v.Quantity,
                    UnitPrice = ArredondarMoeda(v.Menu.PrecoBase)
                });
            }

            return pedido;
        }

        private static string DescricaoCombo(ComboAgrupado c) =>
            $"Menu ({c.Prato.Menu.Nome} + {c.Acompanhamento.Menu.Nome} + {c.Bebida.Menu.Nome})";

        private async Task TentarCancelarAsync(Order pedido)
        {
            try
            {
                pedido.Status = OrderStatus.Cancelado;   // adicionar ao enum se ainda não existir
                await _orderRepository.SaveChangesAsync(); // entidade já tracked pelo DbContext
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Pedido {OrderId} ficou Pendente sem stock descontado — requer reconciliação.", pedido.Id);
            }
        }
    }
}