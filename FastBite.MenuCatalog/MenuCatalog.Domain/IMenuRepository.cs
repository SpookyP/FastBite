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
        Task<IEnumerable<Menu>> ObterTodosAsync();
        Task<Menu> ObterPorIdAsync(int id);
        Task<Menu> AdicionarMenuAsync(Menu menu);
        Task<Menu> AtualizarMenuAsync(Menu menu);
        Task<Menu> RemoverMenuAsync(int id);
    }
}
