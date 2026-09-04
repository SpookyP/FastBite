using MenuCatalog.Domain.Entities;

namespace MenuCatalog.Domain
{
    public interface IItemRepository
    {
        Task<IEnumerable<Item>> GetAllAsync();
        Task<Item> GetByIdAsync(int id);
        Task<Item> AddItemAsync(Item item);
        Task<Item> UpdateItemAsync(Item item);
        Task<Item> DeleteItemAsync(int id);
        Task<IEnumerable<Item>> GetAvailableAsync();
    }
}
