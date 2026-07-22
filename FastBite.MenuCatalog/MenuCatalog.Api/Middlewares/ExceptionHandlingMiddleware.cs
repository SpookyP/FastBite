using System.Net;
using System.Text.Json;

namespace MenuCatalog.Api.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        // O _next representa o "próximo passo" no caminho do pedido HTTP
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // O Gerente abre a porta e deixa o pedido ir para o Controller
                await _next(context);
            }
            catch (Exception ex)
            {
                // Se o Controller ou o Service "rebentarem", o erro volta para trás e é apanhado AQUI!
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // 1. Avisamos que a resposta que vamos devolver é em formato JSON
            context.Response.ContentType = "application/json";

            // 2. Decidimos qual o código de estado HTTP com base no tipo de erro
            context.Response.StatusCode = exception switch
            {
                // Se for o nosso erro de "não encontrado", devolve 404
                KeyNotFoundException => (int)HttpStatusCode.NotFound,

                // Para qualquer outro erro inesperado (ex: base de dados caiu), devolve 500
                _ => (int)HttpStatusCode.InternalServerError
            };

            // 3. Criamos a "Resposta Amigável"
            var response = new
            {
                StatusCode = context.Response.StatusCode,
                // Se for erro 500, escondemos os detalhes técnicos. Se for 404, mostramos a nossa mensagem customizada.
                Message = context.Response.StatusCode == 500
                    ? "Ocorreu um erro interno no servidor. Por favor, tente mais tarde."
                    : exception.Message
            };

            // 4. Transformamos o objeto em JSON e enviamos para o cliente
            var jsonResponse = JsonSerializer.Serialize(response);
            return context.Response.WriteAsync(jsonResponse);
        }
    }
}
