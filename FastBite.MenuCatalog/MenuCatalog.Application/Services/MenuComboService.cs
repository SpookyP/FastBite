.using MenuCatalog.Application.DTOs;
using MenuCatalog.Application.IService;
using MenuCatalog.Domain;
using MenuCatalog.Domain.Entities;

namespace MenuCatalog.Application.Services
{
    public class MenuComboService : IMenuComboService
    {
        private readonly IItemRepository _itemRepository;
        private const decimal DescontoMenu = 0.10m;

        public MenuComboService(IItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }

        public async Task<MenuComboResponseDto> MontarComboAsync(MenuComboCreateDto request)
        {
            var prato = await _itemRepository.GetByIdAsync(request.PratoId);
            var bebida = await _itemRepository.GetByIdAsync(request.BebidaId);
            var acompanhamento = await _itemRepository.GetByIdAsync(request.AcompanhamentoId);

            if (prato == null || bebida == null || acompanhamento == null)
            {
                throw new KeyNotFoundException("Um ou mais itens não existem no catálogo.");
            }

            // Mapear o DTO recebido para a entidade de domínio Menu
            var menuCriado = new MenuCombo(request.Nome, prato, acompanhamento, bebida);

            var precoOriginal = prato.PrecoBase + acompanhamento.PrecoBase + bebida.PrecoBase;
            var precoComDesconto = precoOriginal * (1 - DescontoMenu);

            return new MenuComboResponseDto
            {
                Nome = menuCriado.Nome,
                PratoNome = prato.Nome,
                AcompanhamentoNome = acompanhamento.Nome,
                BebidaNome = bebida.Nome,
                PrecoOriginal = precoOriginal,
                PrecoFinal = precoComDesconto
            };
        }
    }
}
