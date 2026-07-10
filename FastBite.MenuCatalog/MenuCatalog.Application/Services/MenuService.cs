using AutoMapper;
using MenuCatalog.Application.DTOs;
using MenuCatalog.Domain.Entities;

namespace MenuCatalog.Application.Services
{
    public class MenuService
    {
        private readonly IMenuRepository _menuRepository;
        private readonly IMapper _mapper;
        public MenuService(IMenuRepository menuRepository, IMapper mapper)
        {
            _menuRepository = menuRepository;
            _mapper = mapper;
        }

        public async Task<MenuResponseDto> ObterPorIdAsync(int id)
        {
            var menuId = await _menuRepository.ObterPorIdAsync(id);
            if (menuId == null)
            {
                throw new Exception("Menu não encontrado");
            }
            return _mapper.Map<MenuResponseDto>(menuId);
        }

        public async Task<IEnumerable<MenuResponseDto>> ObterTodosAsync()
        {
            var listaMenus = await _menuRepository.GetAllAsync();

            return _mapper.Map<IEnumerable<MenuResponseDto>>(listaMenus);
        }

        public async Task<MenuResponseDto> AdicionarMenuAsync(Menu menu)
        {
            var menuInserido = _mapper.Map<Menu>(menu); // Mapear o objeto Menu para a entidade Menu

            var menuGuardado = await _menuRepository.AdicionarMenuAsync(menuInserido); // Guardar o menu no repositório

            return _mapper.Map<Menu>(menuGuardado); // Mapear a entidade Menu de volta para o objeto Menu e retornar
        }

        public async Task AtualizarMenuAsync(int id, MenuCreateDto request)
        {
            var menuExistente = await _menuRepository.ObterPorIdAsync(id);
            if (menuExistente == null)
            {
                throw new Exception("Menu não encontrado");
            }
            _mapper.Map(request, menuExistente); // Atualizar as propriedades do menu existente com os valores do request

            await _menuRepository.AtualizarMenuAsync(menuExistente);
        }

        public async Task RemoverMenuAsync(int id)
        {
            await _menuRepository.RemoverMenuAsync(id);
        }
    }
}
