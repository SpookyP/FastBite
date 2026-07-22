using AutoMapper;
using MenuCatalog.Application.DTOs;
using MenuCatalog.Application.IService;
using MenuCatalog.Domain;
using MenuCatalog.Domain.Entities;

namespace MenuCatalog.Application.Services
{
    public class MenuService : IMenuService
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
            var menuId = await _menuRepository.GetByIdAsync(id);
            
            return _mapper.Map<MenuResponseDto>(menuId);
        }

        public async Task<IEnumerable<MenuResponseDto>> ObterTodosAsync()
        {
            var listaMenus = await _menuRepository.GetAllAsync();

            return _mapper.Map<IEnumerable<MenuResponseDto>>(listaMenus);
        }

        public async Task<MenuResponseDto> AdicionarMenuAsync(MenuCreateEditDto request)
        {
            // Mapear o DTO recebido para a entidade de domínio Menu
            var menuInserido = _mapper.Map<Menu>(request);

            var menuGuardado = await _menuRepository.AddMenuAsync(menuInserido);

            return _mapper.Map<MenuResponseDto>(menuGuardado);
        }

        public async Task AtualizarMenuAsync(int id, MenuCreateEditDto request)
        {
            var menuExistente = await _menuRepository.GetByIdAsync(id);
            
            _mapper.Map(request, menuExistente); // Atualizar as propriedades do menu existente com os valores do request

            await _menuRepository.UpdateMenuAsync(menuExistente);
        }

        public async Task RemoverMenuAsync(int id)
        {
            await _menuRepository.DeleteMenuAsync(id);
        }

        public async Task<bool> VerDisponibilidadeAsync(int id, int quantidade)
        {
            var menuExistente = await _menuRepository.GetByIdAsync(id);

            if(menuExistente == null)
            {
                return false;
            }

            if(menuExistente.LimiteDiario >= quantidade)
            {
                return true;
            }

            return false;
        }
        public async Task<IEnumerable<MenuResponseDto>> ObterPratosDisponiveisAsync()
        {
            var pratosDisponiveis = await _menuRepository.GetAvailableAsync();

            return _mapper.Map<IEnumerable<MenuResponseDto>>(pratosDisponiveis);
        }
    }
}
