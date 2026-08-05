using MenuCatalog.Api.Middlewares;
using MenuCatalog.Application.IService;
using MenuCatalog.Application.Mapping; // Se o MenuProfile estiver aqui
using MenuCatalog.Application.Services;
using MenuCatalog.Domain;
using MenuCatalog.Infrastructure.Data;
using MenuCatalog.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace MenuCatalog.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Controladores
        builder.Services.AddControllers();

        // Configura��es do Swagger / OpenAPI
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Configura��o do AutoMapper
        builder.Services.AddAutoMapper(config =>
        {
            config.AddMaps(typeof(MenuProfile).Assembly);
        });

        // Configura��o da Base de Dados (Entity Framework)
        builder.Services.AddDbContext<MenuCatalogDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        // Inje��o de Depend�ncias (Servi�os e Reposit�rios)
        builder.Services.AddScoped<IMenuRepository, MenuRepository>();
        builder.Services.AddScoped<IItemService, ItemService>();

        //builder.Services.AddScoped<IMenuComboRepository, MenuRepository>();
       

        // Autentica��o (Ler o Token JWT)
        builder.Services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.Authority = builder.Configuration["JwtSettings:Issuer"];
                options.RequireHttpsMetadata = false;

                options.MapInboundClaims = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidAudience = builder.Configuration["JwtSettings:Audience"],
                    RoleClaimType = "role" // Configura o tipo de claim para roles
                };
            });

        // Autoriza��o (Validar as Regras/Policies)
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("MtoMPolicy", policy => //Pol�tica EXCLUSIVA para a Ordering.API conseguir falar com a MenuCatalog.API
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("scope", "MenuCatalog.api.full");
            });
        });

        // Regra de Apresenta��o (API): Ensinamos a API a permitir liga��es externas (CORS).
        // Colocamos isto aqui na API porque as camadas Application e Domain n�o sabem nem devem saber 
        // o que s�o navegadores web, endere�os HTTP ou seguran�a de redes.

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("PermitirFrontendBlazor", policy =>
            {
                // Para facilitar os nossos testes locais, permitimos pedidos de qualquer origem, 
                // com qualquer cabe�alho e qualquer m�todo (GET, POST, PUT, DELETE).
                policy.AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        var app = builder.Build();

        app.UseMiddleware<ExceptionHandlingMiddleware>();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // IMPORTANTE: O UseCors tem de ficar ANTES do app.UseAuthorization() 
        // e do app.MapControllers(), para que o seguran�a atue logo na entrada do pedido!
        app.UseCors("PermitirFrontendBlazor");

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}