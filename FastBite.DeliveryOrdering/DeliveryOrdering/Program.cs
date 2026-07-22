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

            // Add services to the container.

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));



            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            builder.Services.AddHttpClient();
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = "https://localhost:5001"; // URL
                });

            // Registo dos Repositórios e Serviços da Aplicação
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IOrder, OrderService>();
            builder.Services.AddScoped<IMenuCatalogService, MenuCatalogService>();

            builder.Services.AddAutoMapper(config =>
            {
                config.AddMaps(typeof(OrderProfile).Assembly); // Regista todos os Profiles de AutoMapper no assembly atual
            }); // Regista todos os Profiles de AutoMapper no assembly atual

            var app = builder.Build();

            // Ativa o teu Middleware global de tratamento de erros
            app.UseMiddleware<DeliveryOrdering.API.Middlewares.GlobalExceptionHandlerMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
