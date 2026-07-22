using DeliveryOrdering.Application.Interfaces;
using DeliveryOrdering.Application.Profile;
using DeliveryOrdering.Application.Services;
using DeliveryOrdering.Domain.Interfaces;
using DeliveryOrdering.Infrastructure.Data;
using DeliveryOrdering.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

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
                    options.Authority = "https://localhost:7281";
                    options.RequireHttpsMetadata = false;

                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateAudience = true,
                        ValidateIssuer = true,
                        ValidAudience = "fastbite.order"
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
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            builder.Services.AddHttpClient();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Registo dos Repositórios e Serviços da Aplicação

            builder.Services.AddScoped<IOrderRepository, OrderRepository>();

            builder.Services.AddScoped<IOrder, OrderService>();

            builder.Services.AddScoped<IMenuCatalogService, MenuCatalogService>();


            builder.Services.AddAutoMapper(config =>
            {
                config.AddMaps(typeof(OrderProfile).Assembly); // Regista todos os Profiles de AutoMapper no assembly atual
            });

            var app = builder.Build();

            app.UseMiddleware<API.Middlewares.GlobalExceptionHandlerMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
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
}
