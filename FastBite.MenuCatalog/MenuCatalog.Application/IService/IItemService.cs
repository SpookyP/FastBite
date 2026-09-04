using MenuCatalog.Application.DTOs;


namespace MenuCatalog.Application.IService
{
    public interface IItemService
    {
        Task<ItemResponseDto> ObterPorIdAsync(int id);
        Task<IEnumerable<ItemResponseDto>> ObterTodosAsync();
        Task<ItemResponseDto> AdicionarMenuAsync(ItemCreateEditDto request);
        Task AtualizarMenuAsync(int id, ItemCreateEditDto request);
        Task RemoverMenuAsync(int id);
        Task<bool> VerDisponibilidadeAsync(int id, int quantidade);
        Task<IEnumerable<ItemResponseDto>> ObterPratosDisponiveisAsync();
    }
}
