using DeliveryOrdering.Application.DTOs;
using DeliveryOrdering.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryOrdering.Application.Interfaces
{
    /// <summary>
    /// Serviço responsável pelo processamento e gestão de pedidos (encomendas).
    /// </summary>
    public interface IOrder
    {
        /// <summary>
        /// Cria um pedido a partir de uma lista única de itens. Forma combos
        /// automaticamente (prato + acompanhamento + bebida mais caros disponíveis,
        /// com 10% de desconto) e trata o restante como itens avulsos.
        /// </summary>
        /// <param name="dto">Os dados fornecidos para a criação do pedido, incluindo a lista de itens.</param>
        /// <param name="userId">O identificador único do utilizador (cliente) que está a fazer a encomenda.</param>
        /// <returns>Retorna os detalhes do pedido recém-criado, ou <c>null</c> caso a operação não seja concluída.</returns>
        Task<OrderHistoryResponseDto?> CriarPedidoAsync(CreateOrderRequestDto dto, string userId);

        /// <summary>
        /// Simula o carrinho de compras, agrupando a lista de itens fornecida em combos (menus) 
        /// e itens avulsos, calculando os respetivos totais e descontos. 
        /// Não regista o pedido na base de dados nem altera stocks.
        /// </summary>
        /// <param name="dto">Os dados dos itens a agrupar e calcular.</param>
        /// <returns>Retorna uma estrutura com os itens organizados (combos e avulsos) e o valor total calculado.</returns>
        Task<AgrupamentoCarrinhoResponseDto> AgruparItemsAsync(AgruparItemsRequestDto dto);

        /// <summary>
        /// Obtém o histórico completo de encomendas realizadas por um determinado utilizador.
        /// </summary>
        /// <param name="userId">O identificador único do utilizador.</param>
        /// <returns>Retorna uma lista de pedidos correspondente ao histórico do utilizador.</returns>
        Task<IEnumerable<OrderHistoryResponseDto>> GetUserOrderHistoryAsync(string userId);
    }
}
