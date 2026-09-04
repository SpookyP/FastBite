using DeliveryOrdering.Application.DTOs;
using DeliveryOrdering.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryOrdering.Application.Interfaces
{
    public interface IOrder
    {
        /// <summary>
        /// Criar pedido com itens avulsos
        /// </summary>
        Task<OrderHistoryResponseDto?> CriarPedidoComItensAsync(CreateOrderRequestDto dto, string userId);

        /// <summary>
        /// Criar pedido com combos
        /// </summary>
        Task<OrderHistoryResponseDto?> CriarPedidoComCombosAsync(CreateComboOrderRequestDto dto, string userId);

        /// <summary>
        /// Obter histórico de pedidos por UserId
        /// </summary>
        Task<IEnumerable<OrderHistoryResponseDto>> GetUserOrderHistoryAsync(string userId);
    }
}
