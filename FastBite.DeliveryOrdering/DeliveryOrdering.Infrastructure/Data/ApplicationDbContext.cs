using DeliveryOrdering.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryOrdering.Infrastructure.Data
{
    /// <summary>
    /// Contexto de acesso à base de dados da DeliveryOrdering.API via Entity Framework Core.
    /// Mapeia as entidades Order e OrderItem e a relação 1:N entre elas.
    /// </summary>
    public class ApplicationDbContext : DbContext 
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {

        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}
