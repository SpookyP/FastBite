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
        /// Cria um pedido a partir de uma lista única de itens. Forma combos
        /// automaticamente (prato+acompanhamento+bebida mais caros disponíveis,
        /// com 10% de desconto) e trata o restante como itens avulsos.
        /// </summary>
        Task<OrderHistoryResponseDto?> CriarPedidoAsync(CreateOrderRequestDto dto, string userId);

        Task<AgrupamentoCarrinhoResponseDto> AgruparItemsAsync(AgruparItemsRequestDto dto);

        Task<IEnumerable<OrderHistoryResponseDto>> GetUserOrderHistoryAsync(string userId);
    }
}
