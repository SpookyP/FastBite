using DeliveryOrdering.Application.Interfaces;
using DeliveryOrdering.Application.Profile;
using DeliveryOrdering.Application.Services;
using DeliveryOrdering.Domain.Interfaces;
using DeliveryOrdering.Infrastructure.Data;
using DeliveryOrdering.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace DeliveryOrdering
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = builder.Configuration["JwtSettings:Issuer"];
                    options.RequireHttpsMetadata = false;

                    // Igual ao MenuCatalog: não mapear as claims curtas (sub, role, etc.)
                    // para os tipos longos do .NET, senão o GetUserId e o [Authorize(Roles=...)]
                    // deixam de bater com o que vem no token.
                    options.MapInboundClaims = false;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = true,
                        ValidateIssuer = true,
                        ValidAudience = builder.Configuration["JwtSettings:Audience"],
                        RoleClaimType = "role"   // ← necessário para [Authorize(Roles = "Admin,Client")]
                    };
                });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("OrderAdminPolicy", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", "DeliveryOrdering.api.full");
                });
            });

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddHttpClient<IMenuCatalogService, MenuCatalogService>(client =>
            {
                client.BaseAddress = new Uri(builder.Configuration["ApiUrls:MenuCatalogApi"]);
            });

            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IOrder, OrderService>();

            builder.Services.AddAutoMapper(config =>
            {
                config.AddMaps(typeof(OrderProfile).Assembly);
            });

            // CORS igual ao MenuCatalog (funciona com Bearer no header, sem cookies)
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

            app.UseMiddleware<API.Middlewares.GlobalExceptionHandlerMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Ordem correta: CORS antes do HttpsRedirection/Auth
            app.UseCors("PermitirFrontendBlazor");

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}