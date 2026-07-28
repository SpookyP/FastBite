using FastBite.Frontend.Components;
using FastBite.Frontend.Handlers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace FastBite.Frontend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Forces cookie over HTTPS
                options.Cookie.SameSite = SameSiteMode.Strict; // CSRF 
            })
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                options.Authority = builder.Configuration["JwtSettings:Authority"];
                options.ClientId = builder.Configuration["JwtSettings:ClientId"];
                options.ClientSecret = builder.Configuration["JwtSettings:ClientSecret"];

                options.ResponseType = "code";
                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;

                options.SignedOutCallbackPath = new PathString("/signout-callback-oidc");
                options.SignedOutRedirectUri = "/";

                options.Scope.Clear();
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("email");
                options.Scope.Add(builder.Configuration["JwtSettings:ApiOrderScope"] ?? "");
                options.Scope.Add(builder.Configuration["JwtSettings:ApiMenuScope"] ?? "");
            });

            builder.Services.AddAuthorization();
            builder.Services.AddCascadingAuthenticationState();

            // Permite que o nosso TokenHandler consiga ler os cookies e sessão do utilizador atual
            builder.Services.AddHttpContextAccessor();

            // Registamos o nosso "colador de crachás" na injeção de dependências
            builder.Services.AddTransient<TokenHandler>();

            //builder.Services.AddScoped(sp => new HttpClient
            //{
            //    BaseAddress = new Uri(builder.Configuration["JwtSettings:MenuApiAddress"]??"")
            //});

            builder.Services.AddHttpClient("MenuApi", client =>
            { 
                client.BaseAddress = new Uri(builder.Configuration["JwtSettings:MenuApiAddress"] ?? "");
            }).AddHttpMessageHandler<TokenHandler>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapGet("/logout", () => Results.SignOut(
                new AuthenticationProperties
                {
                    RedirectUri = "/"
                },
                [
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    OpenIdConnectDefaults.AuthenticationScheme
                ]));

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
