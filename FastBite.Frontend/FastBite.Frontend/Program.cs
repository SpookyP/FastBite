            app.MapRazorComponents<App>()
using FastBite.Frontend.Components;
using FastBite.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace FastBite.Frontend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. Configurar Autenticação OIDC + Cookies
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                // O endereço do teu FastBite.Identity.Api
                options.Authority = "https://localhost:7281";

                options.ClientId = "fastbite.frontend";
                options.ClientSecret = "segredo_super_secreto"; // O mesmo definido no Config.cs do Identity
                options.ResponseType = "code";
                options.UsePkce = true;

                options.SaveTokens = true; // Para poderes ler o Access Token mais tarde e enviar para a Menu/Order API

                // Os scopes que definiste no Config.cs
                options.Scope.Clear();
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("email");
                options.Scope.Add("roles");
                options.Scope.Add("MenuCatalog.api.full");
                options.Scope.Add("DeliveryOrdering.api.full");

                options.MapInboundClaims = false; // Preserva as claims originais do JWT (como sub, role, etc.)
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name",
                    RoleClaimType = "role"
                };
            });

            builder.Services.AddCascadingAuthenticationState(); // OBRIGATÓRIO para Blazor reconhecer a sessão

            var app = builder.Build();

            // 2. Registar os Middlewares na ordem correta
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication(); // OBRIGATÓRIO antes do UseAuthorization
            app.UseAuthorization();

            app.UseAntiforgery();

            app.MapRazorComponents<App>()
               .AddInteractiveServerRenderMode(); // ou .AddInteractiveWebAssemblyRenderMode() conforme o teu projeto

            app.Run();
        }
    }
}
