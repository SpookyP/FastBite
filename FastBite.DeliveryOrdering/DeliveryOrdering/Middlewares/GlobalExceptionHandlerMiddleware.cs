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
                // Deixa o pedido seguir normalmente para os Controllers e Services
                await _next(context);
            }
            catch (Exception ex)
            {
                // Se algum erro acontecer (seja de base de dados, regras de negócio ou de comunicação) cai aqui
                _logger.LogError(ex, "Ocorreu um erro na aplicação.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // O tipo de conteúdo padrão para o formato ProblemDetails
            context.Response.ContentType = "application/problem+json";

            // Define o status code como 500 Internal Server Error por defeito
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            // Cria o objeto ProblemDetails exigido pelo enunciado
            var problemDetails = new ProblemDetails
            {
                Status = context.Response.StatusCode,
                Title = "Ocorreu um problema ao processar o pedido.",
                Detail = exception.Message,
                Instance = context.Request.Path
            };

            // Converte o objeto para JSON
            var result = JsonSerializer.Serialize(problemDetails);

            // Escreve a resposta para o cliente
            return context.Response.WriteAsync(result);
        }
    }
}
