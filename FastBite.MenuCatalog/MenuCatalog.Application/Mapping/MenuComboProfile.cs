using AutoMapper;
using MenuCatalog.Application.DTOs;
using MenuCatalog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenuCatalog.Application.Mapping
{
    public class MenuComboProfile : Profile
    {
        public MenuComboProfile()
        {
            CreateMap<MenuComboCreateDto, MenuCombo>();

            CreateMap<MenuCombo, MenuComboResponseDto>()
                .ForMember(dest => dest.PratoNome,
                    opt => opt.MapFrom(src => src.Prato.Nome))
                .ForMember(dest => dest.AcompanhamentoNome,
                    opt => opt.MapFrom(src => src.Acompanhamento.Nome))
                .ForMember(dest => dest.BebidaNome,
                    opt => opt.MapFrom(src => src.Bebida.Nome));
        }
    }
}
