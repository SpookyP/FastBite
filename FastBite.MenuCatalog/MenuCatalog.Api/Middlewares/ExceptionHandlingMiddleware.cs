using System.Net;
using System.Text.Json;

namespace MenuCatalog.Api.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            context.Response.StatusCode = exception switch
            {
                KeyNotFoundException => (int)HttpStatusCode.NotFound,       // 404 - Item não encontrado na BD
                ArgumentException => (int)HttpStatusCode.BadRequest,        // 400 - Categoria inválida no MenuCombo
                _ => (int)HttpStatusCode.InternalServerError                // 500 - Erro inesperado
            };

            var response = new
            {
                StatusCode = context.Response.StatusCode,
                // Mostra a mensagem do erro para 404 e 400; esconde detalhes técnicos para 500
                Message = context.Response.StatusCode == (int)HttpStatusCode.InternalServerError
                    ? "Ocorreu um erro interno no servidor. Por favor, tente mais tarde."
                    : exception.Message
            };

            var jsonResponse = JsonSerializer.Serialize(response);
            return context.Response.WriteAsync(jsonResponse);
        }
    }
}