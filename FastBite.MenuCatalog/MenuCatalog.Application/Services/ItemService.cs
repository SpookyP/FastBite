using AutoMapper;
using MenuCatalog.Application.DTOs;
using MenuCatalog.Application.IService;
using MenuCatalog.Domain;
using MenuCatalog.Domain.Entities;

namespace MenuCatalog.Application.Services
{
    public class ItemService : IItemService
    {
        private readonly IItemRepository _itemRepository;
        private readonly IMapper _mapper;
        public ItemService(IItemRepository itemRepository, IMapper mapper)
        {
            _itemRepository = itemRepository;
            _mapper = mapper;
        }

        public async Task<ItemResponseDto> ObterPorIdAsync(int id)
        {
            var menuId = await _itemRepository.GetByIdAsync(id);
            
            return _mapper.Map<ItemResponseDto>(menuId);
        }

        public async Task<IEnumerable<ItemResponseDto>> ObterTodosAsync()
        {
            var listaMenus = await _itemRepository.GetAllAsync();

            return _mapper.Map<IEnumerable<ItemResponseDto>>(listaMenus);
        }

        public async Task<ItemResponseDto> AdicionarMenuAsync(ItemCreateEditDto request)
        {
            // Mapear o DTO recebido para a entidade de domínio Menu
            var menuInserido = _mapper.Map<Item>(request);

            var menuGuardado = await _itemRepository.AddItemAsync(menuInserido);

            return _mapper.Map<ItemResponseDto>(menuGuardado);
        }

        public async Task AtualizarMenuAsync(int id, ItemCreateEditDto request)
        {
            var menuExistente = await _itemRepository.GetByIdAsync(id);

            if (menuExistente == null)
            {
                throw new KeyNotFoundException($"O item com o ID {id} não foi encontrado.");
            }

            _mapper.Map(request, menuExistente); // Atualizar as propriedades do menu existente com os valores do request

            await _itemRepository.UpdateItemAsync(menuExistente);
        }

        public async Task RemoverMenuAsync(int id)
        {
            var menuExistente = await _itemRepository.GetByIdAsync(id);

            if (menuExistente == null)
            {
                throw new KeyNotFoundException($"O item com o ID {id} não foi encontrado.");
            }

            await _itemRepository.DeleteItemAsync(id);
        }

        public async Task<bool> VerDisponibilidadeAsync(int id, int quantidade)
        {
            var menuExistente = await _itemRepository.GetByIdAsync(id);

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
            var pratosDisponiveis = await _itemRepository.GetAvailableAsync();

            return _mapper.Map<IEnumerable<ItemResponseDto>>(pratosDisponiveis);
        }
    }
}

