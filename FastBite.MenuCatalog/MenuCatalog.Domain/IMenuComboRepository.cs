using MenuCatalog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenuCatalog.Domain
{
    public interface IMenuComboRepository
    {
        Task<IEnumerable<MenuCombo>> GetAllAsync();
        Task<Item> GetByIdAsync(int id);
        Task<Item> AddMenuAsync(MenuCombo menu);
        Task<Item> UpdateDrinkAsync(MenuCombo menu, Item item);
        Task<Item> UpdateSideDishAsync(MenuCombo menu, Item item);
        Task<Item> DeleteMenuAsync(int id);
        Task<IEnumerable<MenuCombo>> GetAvailableAsync();
    }
}
