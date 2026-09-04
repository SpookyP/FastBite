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
                Items = new List<OrderItem>()
            };

            decimal totalAcumulado = 0;

            // Validação de cada combo do pedido
            foreach (var comboDto in dto.Items)
            {
                // Validar que tem todos os IDs necessários para um combo
                if (comboDto.Quantity <= 0 || !comboDto.AcompanhamentoId.HasValue || !comboDto.BebidaId.HasValue)
                    return null;

                // Validar disponibilidade de todos os itens do combo
                bool pratoDiponivel = await _catalogService.VerificarDisponibilidadeAsync(comboDto.ProductId, comboDto.Quantity);
                if (!pratoDiponivel)
                    return null;

                bool acompanhamentoDiponivel = await _catalogService.VerificarDisponibilidadeAsync(comboDto.AcompanhamentoId.Value, comboDto.Quantity);
                if (!acompanhamentoDiponivel)
                    return null;

                bool bebidaDisponivel = await _catalogService.VerificarDisponibilidadeAsync(comboDto.BebidaId.Value, comboDto.Quantity);
                if (!bebidaDisponivel)
                    return null;

                // Obter preço do prato, acompanhamento e bebida
                var prato = await _catalogService.ObterMenuPorIdAsync(comboDto.ProductId);
                if (prato == null)
                    return null;

                var acompanhamento = await _catalogService.ObterMenuPorIdAsync(comboDto.AcompanhamentoId.Value);
                if (acompanhamento == null)
                    return null;

                var bebida = await _catalogService.ObterMenuPorIdAsync(comboDto.BebidaId.Value);
                if (bebida == null)
                    return null;

                // Calcular preço total do combo (soma dos três itens)
                decimal precoCombo = (prato.PrecoBase + acompanhamento.PrecoBase + bebida.PrecoBase) * comboDto.Quantity;
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
