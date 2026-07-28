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

        /// <summary>
        /// Adiciona um novo pedido ao contexto
        /// </summary>
        public async Task OrderCreateAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
        }
        /// <summary>
        /// Devolve todos os pedidos de um utilizador, com os itens incluídos, do mais recente para o mais antigo.
        /// </summary>
        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId) 
        {
            return await _context.Orders
                .Include(order => order.Items) // Traz os pratos associados ao pedido (Relação 1:N)
                .Where(order => order.UserId == userId) // Filtra apenas os pedidos deste utilizador (RF9)
                .OrderByDescending(order => order.OrderDate) // Ordena do mais recente para o mais antigo
                .ToListAsync();
        }
        /// <summary>
        /// Guarda na base de dados todas as alterações pendentes no contexto.
        /// </summary>
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
