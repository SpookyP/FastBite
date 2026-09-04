using AutoMapper;
using DeliveryOrdering.Application.DTOs;
using DeliveryOrdering.Application.Interfaces;
using DeliveryOrdering.Domain.Entities;
using DeliveryOrdering.Domain.Interfaces;
using System;
using System.Collections.Generic;
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

        // Construtor da classe OrderService, que recebe as dependências necessárias
        public OrderService(IMenuCatalogService catalogService, IOrderRepository orderRepository, IMapper mapper)
        {
            _catalogService = catalogService;
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Método para criar um pedido com itens avulsos
        /// </summary>
        public async Task<OrderHistoryResponseDto?> CriarPedidoComItensAsync(CreateOrderRequestDto dto, string userId)
        {
            if (dto?.Items == null || dto.Items.Count == 0)
                return null;

            var novoPedido = new Order
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pendente,
                TotalAmount = 0,
                OrderType = OrderType.Avulso,
                Items = new List<OrderItem>()
            };

            decimal totalAcumulado = 0;

            // Validação de cada item do pedido
            foreach (var itemDto in dto.Items)
            {
                if (itemDto.Quantity <= 0)
                    return null;

                bool disponivel = await _catalogService.VerificarDisponibilidadeAsync(itemDto.ProductId, itemDto.Quantity);
                if (!disponivel)
                    return null;

                var menu = await _catalogService.ObterMenuPorIdAsync(itemDto.ProductId);
                if (menu == null)
                    return null;

                decimal custoDoItem = menu.PrecoBase * itemDto.Quantity;
                totalAcumulado += custoDoItem;

                var orderItem = new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = novoPedido.Id,
                    ProductId = itemDto.ProductId,
                    Quantity = itemDto.Quantity,
                    UnitPrice = menu.PrecoBase
                };

                novoPedido.Items.Add(orderItem);
            }

            novoPedido.TotalAmount = totalAcumulado;

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

            var novoPedido = new Order
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pendente,
                TotalAmount = 0,
                OrderType = OrderType.Combo,
                Items = new List<OrderItem>()
            };

            decimal totalAcumulado = 0;

            // Validação de cada combo do pedido
            foreach (var comboDto in dto.Items)
            {
                if (comboDto.Quantity <= 0 || !comboDto.AcompanhamentoId.HasValue || !comboDto.BebidaId.HasValue)
                    return null;

                // Obter prato
                var prato = await _catalogService.ObterMenuPorIdAsync(comboDto.ProductId);
                if (prato == null)
                    return null;

                // Validar que é Prato
                if (string.IsNullOrEmpty(prato.Categoria) || !prato.Categoria.Equals("Prato", StringComparison.OrdinalIgnoreCase))
                    return null;  // Erro: ProductId não é um Prato

                // Validar disponibilidade do prato
                bool pratoDiponivel = await _catalogService.VerificarDisponibilidadeAsync(comboDto.ProductId, comboDto.Quantity);
                if (!pratoDiponivel)
                    return null;

                // Obter acompanhamento
                var acompanhamento = await _catalogService.ObterMenuPorIdAsync(comboDto.AcompanhamentoId.Value);
                if (acompanhamento == null)
                    return null;

                // Validar que é Acompanhamento
                if (string.IsNullOrEmpty(acompanhamento.Categoria) || !acompanhamento.Categoria.Equals("Acompanhamento", StringComparison.OrdinalIgnoreCase))
                    return null;  // Erro: AcompanhamentoId não é um Acompanhamento

                // Validar disponibilidade do acompanhamento
                bool acompanhamentoDiponivel = await _catalogService.VerificarDisponibilidadeAsync(comboDto.AcompanhamentoId.Value, comboDto.Quantity);
                if (!acompanhamentoDiponivel)
                    return null;

                // Obter bebida
                var bebida = await _catalogService.ObterMenuPorIdAsync(comboDto.BebidaId.Value);
                if (bebida == null)
                    return null;

                // Validar que é Bebida
                if (string.IsNullOrEmpty(bebida.Categoria) || !bebida.Categoria.Equals("Bebida", StringComparison.OrdinalIgnoreCase))
                    return null;  // Erro: BebidaId não é uma Bebida

                // Validar disponibilidade da bebida
                bool bebidaDisponivel = await _catalogService.VerificarDisponibilidadeAsync(comboDto.BebidaId.Value, comboDto.Quantity);
                if (!bebidaDisponivel)
                    return null;

                // Calcular preço total do combo (soma dos três itens)
                decimal precoCombo = (prato.PrecoBase + acompanhamento.PrecoBase + bebida.PrecoBase) * comboDto.Quantity * 0.90m;
                totalAcumulado += precoCombo;

                // Criar item de pedido para o combo
                var orderItem = new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = novoPedido.Id,
                    ProductId = comboDto.ProductId, // Usar o ID do prato como referência do combo
                    Quantity = comboDto.Quantity,
                    UnitPrice = (prato.PrecoBase + acompanhamento.PrecoBase + bebida.PrecoBase)
                };

                novoPedido.Items.Add(orderItem);
            }

            novoPedido.TotalAmount = totalAcumulado;

            await _orderRepository.AdicionarAsync(novoPedido);
            await _orderRepository.SaveChangesAsync();

            return _mapper.Map<OrderHistoryResponseDto>(novoPedido);
        }

        public async Task<IEnumerable<OrderHistoryResponseDto>> GetUserOrderHistoryAsync(string userId)
        {
            // Recupera o histórico de pedidos do usuario
            var orders = await _orderRepository.GetOrdersByUserIdAsync(userId);

            // Mapeia os pedidos para DTOs de resposta
            var response = _mapper.Map<IEnumerable<OrderHistoryResponseDto>>(orders);

            return response;
        }
    }
}
