using MenuCatalog.Application.DTOs;

namespace MenuCatalog.Application.IService
{
    public interface IMenuComboService
    {
        Task<MenuComboResponseDto> MontarComboAsync(MenuComboCreateDto request);
    }
}

