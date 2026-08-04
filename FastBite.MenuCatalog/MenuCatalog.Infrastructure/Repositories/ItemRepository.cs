using MenuCatalog.Domain;
using MenuCatalog.Domain.Entities;
using MenuCatalog.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<Item> AddMenuAsync(Item item)
        {
            _context.Menus.Add(menu);
            await _context.SaveChangesAsync();
            return await GetByIdAsync(menu.Id);
        }

        public async Task<Item> UpdateMenuAsync(Item menu)
        {
            _context.Menus.Update(menu);
            await _context.SaveChangesAsync();
            return await GetByIdAsync(menu.Id);
        }

        public async Task<Item> DeleteMenuAsync(int id)
        {
            var menuParaApagar = await GetByIdAsync(id);

            if (menuParaApagar != null)
            {
                _context.Menus.Remove(menuParaApagar);
                await _context.SaveChangesAsync();
            }

            return menuParaApagar;
        }
        public async Task<IEnumerable<Item>> GetAvailableAsync()
        {
            return await _context.Menus
                                 .Where(m => m.LimiteDiario > 0)
                                 .ToListAsync();
        }
    }
}
