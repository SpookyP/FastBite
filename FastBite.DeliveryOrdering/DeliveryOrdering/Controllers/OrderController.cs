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

        // Preview do carrinho: agrupa em combos + avulsos, calcula totais.
        // Não grava nem desconta stock. Mantém-se sob [Authorize] da classe.
        [HttpPost("agrupar")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AgrupamentoCarrinhoResponseDto>> Agrupar([FromBody] AgruparItemsRequestDto dto)
            => Ok(await _pedidoService.AgruparItemsAsync(dto));

        // Cria o pedido. O serviço lança exceções tipadas (mapeadas no middleware).
        // Já NÃO devolve null — removido o "if (pedidoCriado == null) return BadRequest".
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