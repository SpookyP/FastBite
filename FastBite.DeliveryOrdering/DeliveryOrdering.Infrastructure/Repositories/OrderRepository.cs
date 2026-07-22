using DeliveryOrdering.Domain.Entities;
using DeliveryOrdering.Domain.Interfaces;
using DeliveryOrdering.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryOrdering.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AdicionarAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId)
        {
            return await _context.Orders
                .Include(o => o.Items) // Traz os pratos associados ao pedido (Relação 1:N)
                .Where(o => o.UserId == userId) // Filtra apenas os pedidos deste utilizador (RF9)
                .OrderByDescending(o => o.OrderDate) // Ordena do mais recente para o mais antigo
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
