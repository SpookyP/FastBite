using System.Net;
using System.Text.Json;

namespace MenuCatalog.Api.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Este middleware captura exceções não tratadas durante o processamento das requisições HTTP e retorna respostas apropriadas com códigos de status HTTP e mensagens de erro em formato JSON.
        /// </summary>
        /// <param name="next"></param>
        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Este método é chamado para processar a requisição HTTP. Ele tenta invocar o próximo middleware na pipeline e captura quaisquer exceções que ocorram, chamando o método HandleExceptionAsync para lidar com elas.
        /// </summary>
        /// <param name="context">O contexto HTTP da requisição.</param>
        /// <returns>Uma tarefa que representa a operação assíncrona.</returns>
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

        /// <summary>
        /// Este método lida com exceções capturadas, definindo o código de status HTTP apropriado e retornando uma resposta JSON com informações sobre o erro.
        /// </summary>
        /// <param name="context">O contexto HTTP da requisição.</param>
        /// <param name="exception">A exceção capturada durante o processamento da requisição.</param>
        /// <returns>Uma tarefa que representa a operação assíncrona.</returns>
        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            context.Response.StatusCode = exception switch
            {
                KeyNotFoundException => (int)HttpStatusCode.NotFound,       // 404 - Item não encontrado na BD
                ArgumentException => (int)HttpStatusCode.BadRequest,        // 400 - Campo inválido ou ausente na requisição do MenuCombo
                _ => (int)HttpStatusCode.InternalServerError                // 500 - Erro inesperado
            };

            var response = new
            {
                StatusCode = context.Response.StatusCode,

                Message = context.Response.StatusCode == (int)HttpStatusCode.InternalServerError // Mostra a mensagem do erro para 404 e 400; esconde detalhes técnicos para 500
                    ? "Ocorreu um erro interno no servidor. Por favor, tente mais tarde."
                    : exception.Message
            };

            var jsonResponse = JsonSerializer.Serialize(response);
            return context.Response.WriteAsync(jsonResponse);
        }
    }
}