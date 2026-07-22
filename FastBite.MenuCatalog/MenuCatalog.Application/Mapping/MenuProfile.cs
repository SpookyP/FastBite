using AutoMapper;
using MenuCatalog.Application.DTOs;
using MenuCatalog.Domain.Entities;


namespace MenuCatalog.Application.Mapping
{
    public class MenuProfile : Profile
    {
        public MenuProfile()
        {
            CreateMap<MenuCreateEditDto, Menu>();

            CreateMap<Menu, MenuResponseDto>()
                .ForMember(dest => dest.Nome,
                    opt => opt.MapFrom(src => src.Nome))
                .ForMember(dest => dest.Descricao,
                    opt => opt.MapFrom(src => src.Descricao))
                .ForMember(dest => dest.Categoria,
                    opt => opt.MapFrom(src => src.Categoria))
                .ForMember(dest => dest.Alergenios,
                    opt => opt.MapFrom(src => src.Alergenios))
                .ForMember(dest => dest.PrecoBase,
                    opt => opt.MapFrom(src => src.PrecoBase))
                .ForMember(dest => dest.LimiteDiario,
                    opt => opt.MapFrom(src => src.LimiteDiario));
        }
    }
}
