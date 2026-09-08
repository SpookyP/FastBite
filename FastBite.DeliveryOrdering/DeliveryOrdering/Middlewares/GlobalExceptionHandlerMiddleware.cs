using DeliveryOrdering.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace DeliveryOrdering.API.Middlewares
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

        public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro na aplicação.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var (status, title) = exception switch
            {
                PedidoInvalidoException => (HttpStatusCode.BadRequest, "Pedido inválido."),
                ProdutoNaoEncontradoException => (HttpStatusCode.NotFound, "Produto não encontrado."),
                StockInsuficienteException => (HttpStatusCode.Conflict, "Stock insuficiente."),
                PrecoAlteradoException => (HttpStatusCode.Conflict, "Preços alterados."),
                DescontoStockFalhouException => (HttpStatusCode.BadGateway, "Falha ao descontar stock."),
                _ => (HttpStatusCode.InternalServerError, "Ocorreu um problema ao processar o pedido.")
            };

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = (int)status;

            var problemDetails = new ProblemDetails
            {
                Status = (int)status,
                Title = title,
                Detail = exception.Message,
                Instance = context.Request.Path
            };

            // Extra fields so the frontend can act (e.g. list of unavailable items, order id).
            switch (exception)
            {
                case StockInsuficienteException e:
                    problemDetails.Extensions["itens"] = e.Itens;
                    break;
                case PrecoAlteradoException e:
                    problemDetails.Extensions["subtotalEsperado"] = e.SubtotalEsperado;
                    problemDetails.Extensions["subtotalActual"] = e.SubtotalActual;
                    break;
                case ProdutoNaoEncontradoException e:
                    problemDetails.Extensions["productId"] = e.ProductId;
                    break;
                case DescontoStockFalhouException e:
                    problemDetails.Extensions["orderId"] = e.OrderId;
                    break;
            }

            var result = JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(result);
        }
    }
}