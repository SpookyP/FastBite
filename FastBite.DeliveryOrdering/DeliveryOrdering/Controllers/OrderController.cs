using DeliveryOrdering.Application.DTOs;
using DeliveryOrdering.Application.Interfaces;
using DeliveryOrdering.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DeliveryOrdering.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrder _pedidoService;

        public OrderController(IOrder pedidoService)
        {
            _pedidoService = pedidoService;
        }

        /// <summary>
        /// Endpoint para criar um pedido com itens avulsos
        /// </summary>
        [HttpPost("items")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "Admin,Client")]
        public async Task<IActionResult> CreateItemOrder([FromBody] CreateOrderRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                         ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Utilizador não identificado ou Token JWT inválido.");
            }

            var pedidoCriado = await _pedidoService.CriarPedidoComItensAsync(request, userId);

            if (pedidoCriado == null)
            {
                return BadRequest("Não foi possível processar o pedido. Verifique se os pratos estão disponíveis e têm doses suficientes.");
            }

            return StatusCode(StatusCodes.Status201Created, pedidoCriado);
        }

        /// <summary>
        /// Endpoint para criar um pedido com combos
        /// </summary>
        [HttpPost("combos")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "Admin,Client")]
        public async Task<IActionResult> CreateComboOrder([FromBody] CreateComboOrderRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                         ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Utilizador não identificado ou Token JWT inválido.");
            }

            var pedidoCriado = await _pedidoService.CriarPedidoComCombosAsync(request, userId);

            if (pedidoCriado == null)
            {
                return BadRequest("Não foi possível processar o pedido com combos. Verifique se todos os itens estão disponíveis.");
            }

            return StatusCode(StatusCodes.Status201Created, pedidoCriado);
        }

        /// <summary>
        /// Obtém o histórico de pedidos do usuário autenticado.
        /// </summary>
        [HttpGet("my-orders")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "Admin,Client")]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                         ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Utilizador não identificado.");
            }

            try
            {
                var history = await _pedidoService.GetUserOrderHistoryAsync(userId);

                return Ok(history);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
