using MenuCatalog.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenuCatalog.Application.IService
{
    public interface IMenuComboService
    {
        Task<MenuComboResponseDto> ObterPorIdAsync(int id);
        Task<IEnumerable<MenuComboResponseDto>> ObterTodosAsync();
        Task<MenuComboResponseDto> CriarMenuComboAsync(MenuComboCreateDto request);
        Task AtualizarBebidaDoMenuAsync(int menuComboId, int novaBebidaId);
        Task AtualizarAcompanhamentoDoMenuAsync(int menuComboId, int novoAcompanhamentoId);
        Task RemoverMenuComboAsync(int id);
        Task<IEnumerable<MenuComboResponseDto>> ObterMenusComboDisponiveisAsync();
        }
    }

