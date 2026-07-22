using DeliveryOrdering.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryOrdering.Domain.Interfaces
{
    public interface IOrderRepository
    {
        Task AdicionarAsync(Order order);   // Método para adicionar um pedido
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId);         // Novo método para o GET
        Task SaveChangesAsync();    // Método para salvar as alterações
    }
}
