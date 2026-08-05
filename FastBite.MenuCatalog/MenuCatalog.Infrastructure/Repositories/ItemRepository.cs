using MenuCatalog.Domain;
using MenuCatalog.Domain.Entities;
using MenuCatalog.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MenuCatalog.Infrastructure.Repositories
{
    public class ItemRepository : IItemRepository
    {
        private readonly MenuCatalogDbContext _context;

        public ItemRepository(MenuCatalogDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Item>> GetAllAsync()
        {
            return await _context.Items.ToListAsync();
        }

        public async Task<Item> GetByIdAsync(int id)
        {
            return await _context.Items.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Item> AddItemAsync(Item item)
        {
            _context.Items.Add(item);
            await _context.SaveChangesAsync();
            return await GetByIdAsync(item.Id);
        }

        public async Task<Item> UpdateItemAsync(Item item)
        {
            _context.Items.Update(item);
            await _context.SaveChangesAsync();
            return await GetByIdAsync(item.Id);
        }

        public async Task<Item> DeleteItemAsync(int id)
        {
            var itemParaApagar = await GetByIdAsync(id);

            if (itemParaApagar != null)
            {
                _context.Items.Remove(itemParaApagar);
                await _context.SaveChangesAsync();
            }

            return itemParaApagar;
        }
        public async Task<IEnumerable<Item>> GetAvailableAsync()
        {
            return await _context.Items
                                 .Where(m => m.LimiteDiario > 0)
                                 .ToListAsync();
        }
    }
}
