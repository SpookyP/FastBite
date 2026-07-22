using DeliveryOrdering.Application.DTOs;
using DeliveryOrdering.Domain.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryOrdering.Application.Profile
{
    public class OrderProfile : AutoMapper.Profile
    {
        public OrderProfile()
        {
            // Mapear a Linha do Pedido (Entidade -> DTO de Resposta)
            CreateMap<OrderItem, OrderItemResponseDto>();

            // Mapear o Pedido Principal (Entidade -> DTO de Resposta)
            CreateMap<Order, OrderHistoryResponseDto>()
                // Devolver o ID do Pedido como "OrderId" no DTO
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Id))
                // Converter o Enum para um texto lido pelo cliente
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            // Nota: Não precisas de mapear os CreateOrderRequestDto para as Entidades aqui, 
            // porque vai ser criada a entidade Order manualmente na lógica de negócio 
            // depois de consultar o catálogo de preços!
        }
    }
}
