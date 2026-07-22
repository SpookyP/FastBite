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
        Task<Order?> CriarPedidoAsync(CreateOrderRequestDto dto, string userId);    // Método para criar um pedido
    }
}
