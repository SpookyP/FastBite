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
        builder.Services.AddOpenApi();

        // Configuração do AutoMapper
        builder.Services.AddAutoMapper(config =>
        {
            config.AddMaps(typeof(MenuProfile).Assembly);
        });

        // Configuração da Base de Dados (Entity Framework)
        builder.Services.AddDbContext<MenuCatalogDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        // Injeção de Dependências (Serviços e Repositórios)
        builder.Services.AddScoped<IMenuRepository, MenuRepository>();
        builder.Services.AddScoped<IMenuService, MenuService>();


        // Autenticação (Ler o Token JWT)
        builder.Services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.Authority = "https://localhost:7281";
                options.RequireHttpsMetadata = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidAudience = "fastbite.menu"
                };
            });

        // Autorização (Validar as Regras/Policies)
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("MenuAdminPolicy", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("scope", "MenuCatalog.api.full");
            });
        });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}