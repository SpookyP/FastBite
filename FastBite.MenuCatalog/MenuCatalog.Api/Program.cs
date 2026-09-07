using MenuCatalog.Api.Middlewares;
using MenuCatalog.Application.IService;
using MenuCatalog.Application.Mapping;
using MenuCatalog.Application.Services;
using MenuCatalog.Domain;
using MenuCatalog.Infrastructure.Data;
using MenuCatalog.Infrastructure.Repositories;
using MenuCatalog.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace MenuCatalog.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();


        builder.Services.AddAutoMapper(config =>
        {
            config.AddMaps(typeof(ItemProfile).Assembly);
        });


        builder.Services.AddDbContext<MenuCatalogDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddScoped<IItemRepository, ItemRepository>();
        builder.Services.AddScoped<IItemService, ItemService>();

        builder.Services.AddScoped<IMenuComboService, MenuComboService>();

        builder.Services.AddHostedService<DailyStockResetService>();


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


        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("MtoMPolicy", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("scope", "MenuCatalog.api.full");
            });
        });


        builder.Services.AddCors(options =>
        {
            options.AddPolicy("PermitirFrontendBlazor", policy =>
            {

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


        app.UseCors("PermitirFrontendBlazor");

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}