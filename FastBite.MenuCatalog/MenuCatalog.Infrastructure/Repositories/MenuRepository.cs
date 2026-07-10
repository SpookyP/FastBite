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

        public async Task<IEnumerable<Menu>> ObterTodosAsync()
        {
            return await _context.Menus.ToListAsync();
        }

        public async Task<Menu> ObterPorIdAsync(int id)
        {
            return await _context.Menus.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Menu> AdicionarMenuAsync(Menu menu)
        {
            _context.Menus.Add(menu);
            await _context.SaveChangesAsync();
            return await ObterPorIdAsync(menu.Id);
        }

        public async Task<Menu> AtualizarMenuAsync(Menu menu)
        {
            _context.Menus.Update(menu);
            await _context.SaveChangesAsync();
            return await ObterPorIdAsync(menu.Id);
        }

        public async Task<Menu> RemoverMenuAsync(int id)
        {
            var menuParaApagar = await ObterPorIdAsync(id);

            if (menuParaApagar != null)
            {
                _context.Menus.Remove(menuParaApagar);
                await _context.SaveChangesAsync();
            }

            return menuParaApagar;
        }
    }
}
