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

        // Configurações do Swagger / OpenAPI
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Configuração do AutoMapper
        builder.Services.AddAutoMapper(config =>
        {
            config.AddMaps(typeof(MenuProfile).Assembly);
        });

        // Configuração da Base de Dados (Entity Framework)
        builder.Services.AddDbContext<MenuCatalogDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        // Injeção de Dependências (Serviços e Repositórios)
        builder.Services.AddScoped<IItemRepository, MenuRepository>();
        builder.Services.AddScoped<IMenuService, MenuService>();

        //builder.Services.AddScoped<IMenuComboRepository, MenuRepository>();
       

        // Autenticação (Ler o Token JWT)
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

        // Autorização (Validar as Regras/Policies)
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("MtoMPolicy", policy => //Política EXCLUSIVA para a Ordering.API conseguir falar com a MenuCatalog.API
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("scope", "MenuCatalog.api.full");
            });
        });

        // Regra de Apresentação (API): Ensinamos a API a permitir ligações externas (CORS).
        // Colocamos isto aqui na API porque as camadas Application e Domain não sabem nem devem saber 
        // o que são navegadores web, endereços HTTP ou segurança de redes.

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("PermitirFrontendBlazor", policy =>
            {
                // Para facilitar os nossos testes locais, permitimos pedidos de qualquer origem, 
                // com qualquer cabeçalho e qualquer método (GET, POST, PUT, DELETE).
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
        // e do app.MapControllers(), para que o segurança atue logo na entrada do pedido!
        app.UseCors("PermitirFrontendBlazor");

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}