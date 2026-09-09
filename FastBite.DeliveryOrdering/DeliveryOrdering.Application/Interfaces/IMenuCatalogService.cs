using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeliveryOrdering.Application.DTOs;
using DeliveryOrdering.Application.Services;

namespace DeliveryOrdering.Application.Interfaces
{
    public interface IMenuCatalogService
    {
        Task<bool> VerificarDisponibilidadeAsync(int id, int quantity);
        Task<MenuResponseDto?> ObterMenuPorIdAsync(int id);
        Task DescontarStockAsync(List<CreateOrderItemDto> itensVendidos);
    }
}
