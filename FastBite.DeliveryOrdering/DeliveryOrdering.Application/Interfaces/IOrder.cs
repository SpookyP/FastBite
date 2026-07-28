using DeliveryOrdering.Application.DTOs;
using DeliveryOrdering.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryOrdering.Application.Interfaces
{
    /// <summary>
    /// criação de pedidos e obtenção do histórico de pedidos de um usuário.
    /// </summary>
    public interface IOrder
    {
        Task<OrderHistoryResponseDto?> CriarPedidoAsync(CreateOrderRequestDto dto, string userId);

        Task<IEnumerable<OrderHistoryResponseDto>> GetUserOrderHistoryAsync(string userId); // Método para obter histórico de pedidos por UserId
    }
}
