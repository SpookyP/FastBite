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

        // Construtor da classe PedidoService, que recebe as dependências necessárias
        public OrderService(IMenuCatalogService catalogService, IOrderRepository orderRepository)
        {
            _catalogService = catalogService;
            _orderRepository = orderRepository;
        }

        // Método para criar um pedido
        public async Task<Order?> CriarPedidoAsync(CreateOrderRequestDto dto, string userId)
        {
            if (dto.Items == null || dto.Items.Count == 0) return null;

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
                if (itemDto.Quantity <= 0) return null; 

                var catalogInfo = await _catalogService.ValidarItemNoCatalogoAsync(itemDto.ProductId, itemDto.Quantity);    // Valida se o item existe no catálogo e se a quantidade é suficiente

                if (catalogInfo == null || !catalogInfo.IsAvailable) return null;

                decimal custoDoItem = catalogInfo.UnitPrice * itemDto.Quantity;
                totalAcumulado += custoDoItem;

                // Criação do item do pedido
                var orderItem = new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = novoPedido.Id,
                    ProductId = itemDto.ProductId,
                    Quantity = itemDto.Quantity,
                    UnitPrice = catalogInfo.UnitPrice
                };

                novoPedido.Items.Add(orderItem);
            }

            novoPedido.TotalAmount = totalAcumulado;

            await _orderRepository.AdicionarAsync(novoPedido);
            await _orderRepository.SaveChangesAsync();

            return novoPedido;
        }
    }
}
