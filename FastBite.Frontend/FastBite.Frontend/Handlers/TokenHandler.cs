using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;

namespace FastBite.Frontend.Handlers
{
    public class TokenHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        // Injetamos o IHttpContextAccessor para podermos ler a sessão atual do utilizador
        // no servidor onde o Blazor está a correr.
        public TokenHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // 1. Verificamos se temos um contexto web ativo (se o utilizador está no navegador)
            var context = _httpContextAccessor.HttpContext;
            if (context != null)
            {
                // 2. O método GetTokenAsync procura automaticamente no Cookie/OIDC o "access_token"
                // (Isto só funciona porque colocaste options.SaveTokens = true no teu Program.cs!)
                var token = await context.GetTokenAsync("access_token");

                // 3. Se encontrarmos um token, colamo-lo no cabeçalho "Authorization: Bearer <token>"
                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
            }

            // 4. Deixamos o pedido seguir viagem normal para a internet (agora já com o crachá anexado!)
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
