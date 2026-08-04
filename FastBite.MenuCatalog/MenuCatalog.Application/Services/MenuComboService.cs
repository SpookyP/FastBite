using AutoMapper;
using MenuCatalog.Application.DTOs;
using MenuCatalog.Application.IService;
using MenuCatalog.Domain;
using MenuCatalog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenuCatalog.Application.Services
{
    public class MenuComboService : IMenuComboService
    {
        private readonly IItemRepository _itemRepository;
        private readonly IMenuComboRepository _menuComboRepository;
        private readonly IMapper _mapper;

        public MenuComboService(IMenuComboRepository menuComboRepository, IItemRepository itemRepository, IMapper mapper)
        {
            _menuComboRepository = menuComboRepository;
            _itemRepository = itemRepository;
            _mapper = mapper;
        }

        public async Task<MenuComboResponseDto> ObterPorIdAsync(int id)
        {
            var menuComboId = await _menuComboRepository.GetByIdAsync(id);

            return _mapper.Map<MenuComboResponseDto>(menuComboId);
        }

        public async Task<IEnumerable<MenuComboResponseDto>> ObterTodosAsync()
        {
            var listaMenus = await _menuComboRepository.GetAllAsync();

            return _mapper.Map<IEnumerable<MenuComboResponseDto>>(listaMenus);
        }

        public async Task<MenuComboResponseDto> CriarMenuComboAsync(MenuComboCreateDto request)
        {
            var prato = await _itemRepository.GetByIdAsync(request.PratoId);
            var bebida = await _itemRepository.GetByIdAsync(request.BebidaId);
            var acompanhamento = await _itemRepository.GetByIdAsync(request.AcompanhamentoId);

            if (prato == null || bebida == null || acompanhamento == null)
            {
                throw new KeyNotFoundException("Um ou mais itens não existem no catálogo.");
            }

            // Mapear o DTO recebido para a entidade de domínio Menu
            var menuInserido = new MenuCombo(request.Nome, prato, acompanhamento, bebida);

            var menuGuardado = await _menuComboRepository.CreateMenuComboAsync(menuInserido);

            return _mapper.Map<MenuComboResponseDto>(menuGuardado);
        }

        public async Task AtualizarBebidaDoMenuAsync(int menuComboId, int novaBebidaId)
        {
            var menuCombo = await _menuComboRepository.GetByIdAsync(menuComboId);
            var novaBebida = await _itemRepository.GetByIdAsync(novaBebidaId);

            if (menuCombo == null)
            {
                throw new KeyNotFoundException($"O menu com o ID {menuComboId} não foi encontrado.");
            }

            if (novaBebida == null)
            {
                throw new KeyNotFoundException($"A bebida com o ID {novaBebidaId} não foi encontrada.");
            }

            menuCombo.AlterarBebida(novaBebida);

            await _menuComboRepository.UpdateDrinkMenuAsync(menuCombo);
        }

        public async Task AtualizarAcompanhamentoDoMenuAsync(int menuComboId, int novoAcompanhamentoId)
        {
            var menuCombo = await _menuComboRepository.GetByIdAsync(menuComboId);
            var novoAcompanhamento = await _itemRepository.GetByIdAsync(novoAcompanhamentoId);

            if (menuCombo == null)
            {
                throw new KeyNotFoundException($"O menu com o ID {menuComboId} não foi encontrado.");
            }

            if (novoAcompanhamento == null)
            {
                throw new KeyNotFoundException($"O acompanhamento com o ID {novoAcompanhamentoId} não foi encontrado.");
            }

            menuCombo.AlterarAcompanhamento(novoAcompanhamento);

            await _menuComboRepository.UpdateSideDishMenuAsync(menuCombo);
        }

        public async Task RemoverMenuComboAsync(int id)
        {
            var menuExistente = await _menuComboRepository.GetByIdAsync(id);

            if (menuExistente == null)
            {
                throw new KeyNotFoundException($"O menu com o ID {id} não foi encontrado.");
            }

            await _menuComboRepository.DeleteMenuComboAsync(id);
        }

        public async Task<IEnumerable<MenuComboResponseDto>> ObterMenusComboDisponiveisAsync()
        {
            var menusDisponiveis = await _menuComboRepository.GetAvailableAsync();

            return _mapper.Map<IEnumerable<MenuComboResponseDto>>(menusDisponiveis);
        }
    }
}
