using AutoMapper;
using MenuCatalog.Application.DTOs;
using MenuCatalog.Application.IService;
using MenuCatalog.Domain;
using MenuCatalog.Domain.Entities;

namespace MenuCatalog.Application.Services
{
    public class MenuComboService : IMenuComboService
    {
        private readonly IItemRepository _itemRepository;
        private readonly IMapper _mapper;
        private const decimal DescontoMenu = 0.10m;

        public MenuComboService(IItemRepository itemRepository, IMapper mapper)
        {
            _itemRepository = itemRepository;
            _mapper = mapper;
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
            if (prato.LimiteDiario <= 0)
            {
                throw new ArgumentException($"O prato '{prato.Nome}' encontra-se esgotado.");
            }

            if (acompanhamento.LimiteDiario <= 0)
            {
                throw new ArgumentException($"O acompanhamento '{acompanhamento.Nome}' encontra-se esgotado.");
            }

            if (bebida.LimiteDiario <= 0)
            {
                throw new ArgumentException($"A bebida '{bebida.Nome}' encontra-se esgotada.");
            }

            // Mapear o DTO recebido para a entidade de domínio Menu
            var menuCriado = new MenuCombo(request.Nome, prato, acompanhamento, bebida);

            var precoOriginal = prato.PrecoBase + acompanhamento.PrecoBase + bebida.PrecoBase;
            var precoComDesconto = precoOriginal * (1 - DescontoMenu);

            var dto = _mapper.Map<MenuComboResponseDto>(menuCriado);

            dto.PrecoOriginal = Math.Round(precoOriginal, 2);
            dto.PrecoFinal = Math.Round(precoComDesconto, 2);

            return dto;
        }
    }
}
