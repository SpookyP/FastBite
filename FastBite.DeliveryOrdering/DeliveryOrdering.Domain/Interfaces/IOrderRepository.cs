using DeliveryOrdering.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryOrdering.Domain.Interfaces
{
    /// <summary>
    /// Contrato de acesso a dados para a entidade Order.
    /// Implementado na camada Infrastructure (OrderRepository), usando EF Core.
    /// </summary>

    public interface IOrderRepository
    {
        Task OrderCreateAsync(Order order);
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId); 
        Task SaveChangesAsync();
    }
}
