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
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IPedido _pedidoService;

        public OrderController(IPedido pedidoService)
        {
            _pedidoService = pedidoService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]    // Em caso de sucesso
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Em caso de erro de validação
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] // Se o usuário não estiver autenticado
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                         ?? User.FindFirst("sub")?.Value;   // Diferentes tipos de tokens podem ter diferentes claims para o ID do usuário

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Utilizador não identificado ou Token JWT inválido.");
            }

            var pedidoCriado = await _pedidoService.CriarPedidoAsync(request, userId);

            if (pedidoCriado == null)
            {
                return BadRequest("Não foi possível processar o pedido. Verifique se os pratos estão disponíveis e têm doses suficientes.");
            }

            return StatusCode(StatusCodes.Status201Created, pedidoCriado);
        }
    }
}
