using MenuCatalog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenuCatalog.Domain
{
    public interface IMenuRepository
    {
        Task<IEnumerable<Menu>> GetAllAsync();
        Task<Menu> GetByIdAsync(int id);
        Task<Menu> AddMenuAsync(Menu menu);
        Task<Menu> UpdateMenuAsync(Menu menu);
        Task<Menu> DeleteMenuAsync(int id);
        Task<IEnumerable<Menu>> GetAvailableAsync();
    }
}
