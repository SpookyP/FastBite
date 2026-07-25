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
        Task<bool> VerificarDisponibilidadeAsync(int id, int quantity);    // Método para validar se o item está disponível no catálogo
        Task<MenuResponseDto?> ObterMenuPorIdAsync(int id);    // Método para obter o preço do item no catálogo
    }
}
