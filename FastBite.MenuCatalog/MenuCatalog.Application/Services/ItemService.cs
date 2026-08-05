using AutoMapper;
using MenuCatalog.Application.DTOs;
using MenuCatalog.Application.IService;
using MenuCatalog.Domain;
using MenuCatalog.Domain.Entities;

namespace MenuCatalog.Application.Services
{
    public class ItemService : IMenuService
    {
        private readonly IItemRepository _menuRepository;
        private readonly IMapper _mapper;
        public ItemService(IMenuRepository menuRepository, IMapper mapper)
        {
            _menuRepository = menuRepository;
            _mapper = mapper;
        }

        public async Task<ItemResponseDto> ObterPorIdAsync(int id)
        {
            var menuId = await _menuRepository.GetByIdAsync(id);
            
            return _mapper.Map<ItemResponseDto>(menuId);
        }

        public async Task<IEnumerable<ItemResponseDto>> ObterTodosAsync()
        {
            var listaMenus = await _menuRepository.GetAllAsync();

            return _mapper.Map<IEnumerable<ItemResponseDto>>(listaMenus);
        }

        public async Task<ItemResponseDto> AdicionarMenuAsync(ItemCreateEditDto request)
        {
            // Mapear o DTO recebido para a entidade de domínio Menu
            var menuInserido = _mapper.Map<Item>(request);

            var menuGuardado = await _menuRepository.AddMenuAsync(menuInserido);

            return _mapper.Map<ItemResponseDto>(menuGuardado);
        }

        public async Task AtualizarMenuAsync(int id, ItemCreateEditDto request)
        {
            var menuExistente = await _menuRepository.GetByIdAsync(id);

            if (menuExistente == null)
            {
                throw new KeyNotFoundException($"O menu com o ID {id} não foi encontrado.");
            }

            _mapper.Map(request, menuExistente); // Atualizar as propriedades do menu existente com os valores do request

            await _menuRepository.UpdateMenuAsync(menuExistente);
        }

        public async Task RemoverMenuAsync(int id)
        {
            var menuExistente = await _menuRepository.GetByIdAsync(id);

            if (menuExistente == null)
            {
                throw new KeyNotFoundException($"O menu com o ID {id} não foi encontrado.");
            }

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

        public async Task<IEnumerable<ItemResponseDto>> ObterPratosDisponiveisAsync()
        {
            var pratosDisponiveis = await _menuRepository.GetAvailableAsync();

            return _mapper.Map<IEnumerable<ItemResponseDto>>(pratosDisponiveis);
        }
    }
}
