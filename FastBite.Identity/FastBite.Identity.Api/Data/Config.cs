using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.DataProtection;
using Secret = Duende.IdentityServer.Models.Secret;

namespace FastBite.Identity.Api.Data
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),

                new IdentityResource("roles","Perfis de Utilizador", new[]{"role"})
            };
        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("api_menu", "FastBite Food Catalog"),
                new ApiScope("api_order", "FastBite Food Ordering"),
            };

        public static IEnumerable<Client> Clients =>
            new Client[] { 
                new Client{
                    ClientId = "web_app",
                    ClientName = "FastBite Online",

                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,

                    ClientSecrets = { new Secret("segredo_super_secreto".Sha256())},

                    RedirectUris = {"https://localhost:5002/signin-oidc"},
                    PostLogoutRedirectUris = {"https://localhost:5002/signout-callback-oidc"},

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "roles",
                        "api_menu",
                        "api_order"
                    },

                    AllowOfflineAccess = true,
                }
            };
    }
}
