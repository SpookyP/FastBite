using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeliveryOrdering.Application.DTOs;
using DeliveryOrdering.Application.Services;

namespace DeliveryOrdering.Application.Interfaces
{
    /// <summary>
    /// Serviço responsável pela gestão do catálogo de produtos e menus.
    /// Fornece operações para consulta de detalhes, validação de disponibilidade e atualização de stock.
    /// </summary>
    public interface IMenuCatalogService
    {
        /// <summary>
        /// Verifica se existe stock (disponibilidade) suficiente para uma determinada quantidade de um menu.
        /// </summary>
        /// <param name="id">O identificador único do menu a verificar.</param>
        /// <param name="quantity">A quantidade pretendida pelo cliente.</param>
        /// <returns>Retorna <c>true</c> se a quantidade pedida estiver disponível em stock; caso contrário, <c>false</c>.</returns>
        Task<bool> VerificarDisponibilidadeAsync(int id, int quantity);

        /// <summary>
        /// Obtém os detalhes de um menu específico através do seu identificador.
        /// </summary>
        /// <param name="id">O identificador único do menu.</param>
        /// <returns>Retorna os detalhes do menu sob a forma de <see cref="MenuResponseDto"/>, ou <c>null</c> se o menu não for encontrado.</returns>
        Task<MenuResponseDto?> ObterMenuPorIdAsync(int id);

        /// <summary>
        /// Deduz do inventário as quantidades dos menus que foram efetivamente vendidos numa encomenda.
        /// </summary>
        /// <param name="itensVendidos">A lista de itens da encomenda, contendo o ID de cada menu e a respetiva quantidade a descontar.</param>
        Task DescontarStockAsync(List<CreateOrderItemDto> itensVendidos);
    }
}
