using DeliveryOrdering.Application.DTOs;
using DeliveryOrdering.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DeliveryOrdering.Controllers
{

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

            if (pedidoCriado == null)
                return BadRequest("Não foi possível processar o pedido. Verifique se os itens estão disponíveis.");

            return StatusCode(StatusCodes.Status201Created, pedidoCriado);
        }

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