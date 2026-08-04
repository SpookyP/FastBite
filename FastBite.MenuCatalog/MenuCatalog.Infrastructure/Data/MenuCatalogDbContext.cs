using MenuCatalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MenuCatalog.Infrastructure.Data
{
    public class MenuCatalogDbContext : DbContext
    {
        public MenuCatalogDbContext(DbContextOptions<MenuCatalogDbContext> options) : base(options)
        {
        }
        public DbSet<Item> Items { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Item>()
            .Property(p => p.PrecoBase)
            .HasColumnType("decimal(18,2)");

        }
    }
}
