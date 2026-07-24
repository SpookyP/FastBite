using System.Security.Claims;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FastBite.Identity.Api.Data
{
    public static class SeedData
    {
        public static async Task EnsureSeedData(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await context.Database.MigrateAsync();
            var userMgr =
                scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleMgr =
                scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var adminRole = "Admin";
            var clienteRole = "Client";
            if (!await roleMgr.RoleExistsAsync(adminRole))
            {
                await roleMgr.CreateAsync(new IdentityRole(adminRole));
            }
            if (!await roleMgr.RoleExistsAsync(clienteRole))
            {
                await roleMgr.CreateAsync(new IdentityRole(clienteRole));
            }
            var baseAdmin = await userMgr.FindByNameAsync("admin");
            if (baseAdmin == null)
            {
                baseAdmin = new IdentityUser
                {
                    UserName = "admin",
                    Email = "admin@fastbite.local",
                    EmailConfirmed = true
                };
                var result = await userMgr.CreateAsync(baseAdmin, "Atec@123!");

                if (!result.Succeeded) throw new Exception(result.Errors.First().Description);

                await userMgr.AddClaimsAsync(baseAdmin, new Claim[]{
                    new Claim(JwtClaimTypes.Name, "Admin FastBite"),
                    new Claim(JwtClaimTypes.GivenName, "Admin"),
                    new Claim(JwtClaimTypes.FamilyName, "FastBite"),
                    new Claim(JwtClaimTypes.WebSite, "http://fastbite.com"),
                });

                await userMgr.AddToRoleAsync(baseAdmin, adminRole);

                Console.WriteLine("Admin criado com sucesso");
            }
            else
            {
                Console.WriteLine("Admin já existe");
            }
            var baseCliente = await userMgr.FindByNameAsync("cliente");
            if (baseCliente == null)
            {
                baseCliente = new IdentityUser
                {
                    UserName = "cliente",
                    Email = "cliente@fastbite.local",
                    EmailConfirmed = true
                };
                var result = await userMgr.CreateAsync(baseCliente, "Atec@123!");

                if (!result.Succeeded) throw new Exception(result.Errors.First().Description);

                await userMgr.AddClaimsAsync(baseCliente, new Claim[]{
                    new Claim(JwtClaimTypes.Name, "Cliente FastBite"),
                    new Claim(JwtClaimTypes.GivenName, "Cliente"),
                    new Claim(JwtClaimTypes.FamilyName, "FastBite"),
                    new Claim(JwtClaimTypes.WebSite, "http://fastbite.com"),
                });

                await userMgr.AddToRoleAsync(baseCliente, clienteRole);

                Console.WriteLine("Cliente criado com sucesso");
            }
            else
            {
                Console.WriteLine("Cliente já existe");
            }
        }
    }
}
