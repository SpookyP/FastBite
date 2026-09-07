using AutoMapper;
using DeliveryOrdering.Application.DTOs;
using DeliveryOrdering.Application.Interfaces;
using DeliveryOrdering.Domain.Entities;
using DeliveryOrdering.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliveryOrdering.Application.Services
{
    public class OrderService : IOrder
    {
        private readonly IMenuCatalogService _catalogService;
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        private const decimal DescontoCombo = 0.10m;

        public OrderService(IMenuCatalogService catalogService, IOrderRepository orderRepository, IMapper mapper)
        {
            _catalogService = catalogService;
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        public async Task<OrderHistoryResponseDto?> CriarPedidoAsync(CreateOrderRequestDto dto, string userId)
        {
            if (dto?.Items == null || dto.Items.Count == 0)
                return null;

            if (dto.Entrega == null || string.IsNullOrEmpty(dto.Entrega.NomeCompleto) ||
                string.IsNullOrEmpty(dto.Entrega.ContactoTelefonico) ||
                string.IsNullOrEmpty(dto.Entrega.Morada) ||
                string.IsNullOrEmpty(dto.Entrega.CodigoPostal) ||
                string.IsNullOrEmpty(dto.Entrega.Cidade))
                return null;
            if (dto.Entrega == null || string.IsNullOrEmpty(dto.Entrega.NomeCompleto) ||
                string.IsNullOrEmpty(dto.Entrega.ContactoTelefonico) ||
                string.IsNullOrEmpty(dto.Entrega.Morada) ||
                string.IsNullOrEmpty(dto.Entrega.CodigoPostal) ||
                string.IsNullOrEmpty(dto.Entrega.Cidade))
                return null;
            if (dto.Pagamento == null || string.IsNullOrEmpty(dto.Pagamento.MetodoPagamento) || dto.Pagamento.TaxaEntrega < 0)
                return null;

            var novoPedido = new Order
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pendente,
                TotalAmount = 0,
                Subtotal = 0,
                OrderType = OrderType.Avulso,
                Items = new List<OrderItem>(),
                NomeCompleto = dto.Entrega.NomeCompleto,
                ContactoTelefonico = dto.Entrega.ContactoTelefonico,
                Morada = dto.Entrega.Morada,
                CodigoPostal = dto.Entrega.CodigoPostal,
                Cidade = dto.Entrega.Cidade,
                MetodoPagamento = dto.Pagamento.MetodoPagamento,
                TaxaEntrega = dto.Pagamento.TaxaEntrega
            };

            if (dto.Pagamento == null || string.IsNullOrEmpty(dto.Pagamento.MetodoPagamento) || dto.Pagamento.TaxaEntrega < 0)
                return null;

            // Passo 1: expandir cada linha em unidades individuais, com dados reais da Catalog
            var unidades = new List<(int ProductId, MenuResponseDto Menu)>();

            foreach (var itemDto in dto.Items)
            {
                if (itemDto.Quantity <= 0)
                    return null;

                var menu = await _catalogService.ObterMenuPorIdAsync(itemDto.ProductId);
                if (menu == null)
                    return null;

                bool disponivel = await _catalogService.VerificarDisponibilidadeAsync(itemDto.ProductId, itemDto.Quantity);
                if (!disponivel)
                    return null;

                for (int i = 0; i < itemDto.Quantity; i++)
                    unidades.Add((itemDto.ProductId, menu));
            }

            // Passo 2: separar por categoria, mais caro primeiro
            var pratos = unidades.Where(u => u.Menu.Categoria.Equals("Prato", StringComparison.OrdinalIgnoreCase))
                                  .OrderByDescending(u => u.Menu.PrecoBase).ToList();
            var acompanhamentos = unidades.Where(u => u.Menu.Categoria.Equals("Acompanhamento", StringComparison.OrdinalIgnoreCase))
                                           .OrderByDescending(u => u.Menu.PrecoBase).ToList();
            var bebidas = unidades.Where(u => u.Menu.Categoria.Equals("Bebida", StringComparison.OrdinalIgnoreCase))
                                   .OrderByDescending(u => u.Menu.PrecoBase).ToList();

            novoPedido.Subtotal = totalAcumulado;
            novoPedido.TotalAmount = totalAcumulado + dto.Pagamento.TaxaEntrega;

            await _orderRepository.AdicionarAsync(novoPedido);
            await _orderRepository.SaveChangesAsync();

            return _mapper.Map<OrderHistoryResponseDto>(novoPedido);
        }

        /// <summary>
        /// Método para criar um pedido com combos
        /// </summary>
        public async Task<OrderHistoryResponseDto?> CriarPedidoComCombosAsync(CreateComboOrderRequestDto dto, string userId)
        {
            if (dto?.Items == null || dto.Items.Count == 0)
                return null;

            if (dto.Entrega == null || string.IsNullOrEmpty(dto.Entrega.NomeCompleto) ||
                string.IsNullOrEmpty(dto.Entrega.ContactoTelefonico) ||
                string.IsNullOrEmpty(dto.Entrega.Morada) ||
                string.IsNullOrEmpty(dto.Entrega.CodigoPostal) ||
                string.IsNullOrEmpty(dto.Entrega.Cidade))
                return null;

            if (dto.Pagamento == null || string.IsNullOrEmpty(dto.Pagamento.MetodoPagamento) || dto.Pagamento.TaxaEntrega < 0)
                return null;


            var novoPedido = new Order
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pendente,
                TotalAmount = 0,
                OrderType = OrderType.Combo,
                Items = new List<OrderItem>(),
                NomeCompleto = dto.Entrega.NomeCompleto,
                ContactoTelefonico = dto.Entrega.ContactoTelefonico,
                Morada = dto.Entrega.Morada,
                CodigoPostal = dto.Entrega.CodigoPostal,
                Cidade = dto.Entrega.Cidade,
                MetodoPagamento = dto.Pagamento.MetodoPagamento,
                TaxaEntrega = dto.Pagamento.TaxaEntrega
            };

            decimal totalAcumulado = 0;

            // Passo 3: formar combos, greedy — mais caro de cada balde
            while (pratos.Count > 0 && acompanhamentos.Count > 0 && bebidas.Count > 0)
            {
                var prato = pratos[0]; pratos.RemoveAt(0);
                var acomp = acompanhamentos[0]; acompanhamentos.RemoveAt(0);
                var bebida = bebidas[0]; bebidas.RemoveAt(0);

                decimal precoOriginal = prato.Menu.PrecoBase + acomp.Menu.PrecoBase + bebida.Menu.PrecoBase;
                decimal precoComDesconto = Math.Round(precoOriginal * (1 - DescontoCombo), 2);
                totalAcumulado += precoComDesconto;

                novoPedido.Items.Add(new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = novoPedido.Id,
                    Type = OrderItemType.Combo,
                    ProductId = prato.ProductId,
                    DescricaoItem = $"Menu ({prato.Menu.Nome} + {acomp.Menu.Nome} + {bebida.Menu.Nome})",
                    AcompanhamentoId = acomp.ProductId,
                    AcompanhamentoNome = acomp.Menu.Nome,
                    BebidaId = bebida.ProductId,
                    BebidaNome = bebida.Menu.Nome,
                    Quantity = 1,
                    UnitPrice = precoComDesconto
                });
            }

            // Passo 4: sobras vão avulso, agrupadas por ProductId
            var restantes = pratos.Concat(acompanhamentos).Concat(bebidas).GroupBy(u => u.ProductId);

            foreach (var grupo in restantes)
            {
                var menu = grupo.First().Menu;
                int qtd = grupo.Count();

                totalAcumulado += menu.PrecoBase * qtd;

                novoPedido.Items.Add(new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = novoPedido.Id,
                    Type = OrderItemType.Avulso,
                    ProductId = grupo.Key,
                    DescricaoItem = menu.Nome,
                    Quantity = qtd,
                    UnitPrice = menu.PrecoBase
                });
            }

            novoPedido.Subtotal = totalAcumulado;
            novoPedido.TotalAmount = totalAcumulado + dto.Pagamento.TaxaEntrega;
            novoPedido.Subtotal = totalAcumulado;
            novoPedido.TotalAmount = totalAcumulado + dto.Pagamento.TaxaEntrega;

            await _orderRepository.AdicionarAsync(novoPedido);
            await _orderRepository.SaveChangesAsync();

            return _mapper.Map<OrderHistoryResponseDto>(novoPedido);
        }

        public async Task<IEnumerable<OrderHistoryResponseDto>> GetUserOrderHistoryAsync(string userId)
        {
            var orders = await _orderRepository.GetOrdersByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<OrderHistoryResponseDto>>(orders);
        }
    }
}