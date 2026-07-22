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

        [HttpGet("my-orders")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value   // Obtém o ID do usuário a partir do token JWT ou pelo claim "sub" (abreviação de subject)
                         ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Utilizador não identificado.");
            }

            try
            {
                // Chama o serviço que preparaste
                var history = await _pedidoService.GetUserOrderHistoryAsync(userId);

                // Retorna 200 OK com os DTOs
                return Ok(history);
            }
            catch (Exception ex)
            {
                // Qualquer problema será apanhado pelo teu GlobalExceptionHandlerMiddleware
                throw;
            }
        }
    }
}
