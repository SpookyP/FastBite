using DeliveryOrdering.Application.DTOs;
using DeliveryOrdering.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DeliveryOrdering.Controllers
{
    /// <summary>
    /// Controlador responsável pela gestão de pedidos (encomendas) dos clientes.
    /// Permite agrupar itens do carrinho, criar novos pedidos e consultar o histórico.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Client")]
    public class OrderController : ControllerBase
    {
        private readonly IOrder _pedidoService;

        public OrderController(IOrder pedidoService)
        {
            _pedidoService = pedidoService;
        }

        private string? GetUserId() =>
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;

        /// <summary>
        /// Pré-visualiza o carrinho de compras agrupando os itens e calculando os totais.
        /// </summary>
        /// <remarks>
        /// Este endpoint permite simular o carrinho, agrupando os produtos em combos (menus) ou mantendo-os avulsos.
        /// Calcula o valor total a pagar, mas **não regista a encomenda** na base de dados nem desconta o stock.
        /// </remarks>
        /// <param name="dto">Os dados dos itens a agrupar e simular no carrinho.</param>
        /// <returns>Retorna a estrutura do carrinho com os itens agrupados e os totais calculados.</returns>
        /// <response code="200">Carrinho agrupado e calculado com sucesso.</response>
        /// <response code="400">Os dados fornecidos no pedido são inválidos.</response>
        [HttpPost("agrupar")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AgrupamentoCarrinhoResponseDto>> Agrupar([FromBody] AgruparItemsRequestDto dto)
            => Ok(await _pedidoService.AgruparItemsAsync(dto));

        /// <summary>
        /// Cria e regista um novo pedido (encomenda) no sistema.
        /// </summary>
        /// <remarks>
        /// O pedido é automaticamente associado ao utilizador autenticado que faz o pedido.
        /// Valida as regras de negócio de criação e desconta o stock (se aplicável).
        /// </remarks>
        /// <param name="request">Os dados necessários para a criação do pedido.</param>
        /// <returns>Retorna os detalhes do pedido recém-criado.</returns>
        /// <response code="201">Pedido criado com sucesso.</response>
        /// <response code="400">Os dados enviados são inválidos (ex: ModelState inválido).</response>
        /// <response code="401">Utilizador não identificado ou Token JWT inválido.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Utilizador não identificado ou Token JWT inválido.");

            var pedidoCriado = await _pedidoService.CriarPedidoAsync(request, userId);
            return StatusCode(StatusCodes.Status201Created, pedidoCriado);
        }

        /// <summary>
        /// Obtém o histórico de pedidos do utilizador autenticado.
        /// </summary>
        /// <remarks>
        /// Devolve uma lista com todas as encomendas passadas feitas pelo utilizador que detém o Token JWT atual.
        /// </remarks>
        /// <returns>Uma lista contendo o histórico de pedidos do utilizador.</returns>
        /// <response code="200">Histórico de pedidos devolvido com sucesso.</response>
        /// <response code="401">Utilizador não identificado ou Token JWT inválido.</response>
        [HttpGet("my-orders")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Utilizador não identificado.");

            var history = await _pedidoService.GetUserOrderHistoryAsync(userId);
            return Ok(history);
        }
    }
}