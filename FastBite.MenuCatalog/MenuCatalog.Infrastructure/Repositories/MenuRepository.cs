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
    public class MenuRepository : IMenuRepository
    {
        private readonly MenuCatalogDbContext _context;

        public MenuRepository(MenuCatalogDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Menu>> GetAllAsync()
        {
            return await _context.Menus.ToListAsync();
        }

        public async Task<Menu> GetByIdAsync(int id)
        {
            return await _context.Menus.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Menu> AddMenuAsync(Menu menu)
        {
            _context.Menus.Add(menu);
            await _context.SaveChangesAsync();
            return await GetByIdAsync(menu.Id);
        }

        public async Task<Menu> UpdateMenuAsync(Menu menu)
        {
            _context.Menus.Update(menu);
            await _context.SaveChangesAsync();
            return await GetByIdAsync(menu.Id);
        }

        public async Task<Menu> DeleteMenuAsync(int id)
        {
            var menuParaApagar = await GetByIdAsync(id);

            if (menuParaApagar != null)
            {
                _context.Menus.Remove(menuParaApagar);
                await _context.SaveChangesAsync();
            }

            return menuParaApagar;
        }
    }
}
