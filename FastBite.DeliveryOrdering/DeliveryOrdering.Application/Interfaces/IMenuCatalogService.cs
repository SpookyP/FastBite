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
        /// <summary>
        /// Validar se um item está disponível no catálogo
        /// </summary>
        Task<bool> VerificarDisponibilidadeAsync(int id, int quantity);

        /// <summary>
        /// Obter detalhes e preço de um item
        /// </summary>
        Task<MenuResponseDto?> ObterMenuPorIdAsync(int id);

        /// <summary>
        /// Validar disponibilidade e obter preço de um combo
        /// </summary>
        Task<decimal> VerificarDisponibilidadeComboAsync(int pratoId, int acompanhamentoId, int bebidaId, int quantity);
    }
}
