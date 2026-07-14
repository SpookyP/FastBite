using MenuCatalog.Application.DTOs;
using MenuCatalog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenuCatalog.Application.IService
{
    public interface IMenuService
    {
        Task<MenuResponseDto> ObterPorIdAsync(int id);
        Task<IEnumerable<MenuResponseDto>> ObterTodosAsync();
        Task<MenuResponseDto> AdicionarMenuAsync(Menu menu);
        Task AtualizarMenuAsync(int id, MenuCreateEditDto request);
        Task RemoverMenuAsync(int id);
    }
}
