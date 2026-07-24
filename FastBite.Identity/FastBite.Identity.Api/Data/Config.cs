using Duende.IdentityServer;
using Duende.IdentityServer.Models;
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
                new ApiScope("MenuCatalog.api.full", "FastBite Food Catalog"),
                new ApiScope("DeliveryOrdering.api.full", "FastBite Food Ordering"),
            };

        public static IEnumerable<ApiResource> ApiResources =>
            new ApiResource[]
            {
                new ApiResource("fastbite.menu", "FastBite Menu API")
                {
                    Scopes = { "MenuCatalog.api.full" },
                },
                new ApiResource("fastbite.order", "FastBite Order API")
                {
                    Scopes = { "DeliveryOrdering.api.full" },
                },
            };

        public static IEnumerable<Client> GetClients(IConfiguration configuration)
        {
            var clientSecret = configuration["JwtSettings:Secret"] ?? throw new InvalidOperationException("Client secret is missing from configuration!");
            var redirectUris = configuration["JwtSettings:RedirectUri"] ?? throw new InvalidOperationException("RedirectUri is missing from configuration!");
            var postLogoutRedirectUris = configuration["JwtSettings:PostLogoutRedirectUri"] ?? throw new InvalidOperationException("PostLogoutRedirectUri is missing from configuration!");
            var clientOrigin = configuration["JwtSettings:ClientOrigin"] ?? throw new InvalidOperationException("ClientOrigin is missing from configuration!");

            int tokenLifetime = int.TryParse(configuration["JwtSettings:ExpirationInMinutes"], out var lifetime) ? lifetime * 60 : 3600;

            return new Client[] {
                new Client{
                    ClientId = "fastbite.web",
                    ClientName = "FastBite Web App",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireConsent = false,
                    RequireClientSecret = false,
                    RedirectUris = { redirectUris },
                    PostLogoutRedirectUris = { postLogoutRedirectUris },
                    AllowedCorsOrigins = { clientOrigin },
                    AccessTokenLifetime = tokenLifetime,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "roles",
                        "MenuCatalog.api.full",
                        "DeliveryOrdering.api.full"
                    },
                    AllowOfflineAccess = true,
                },
                new Client{
                    ClientId = "api.test.user",
                    ClientName = "Bruno User CLient",

                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                    ClientSecrets = { new Secret(clientSecret.Sha256())},

                    AccessTokenLifetime = tokenLifetime,

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "roles",
                        "MenuCatalog.api.full",
                        "DeliveryOrdering.api.full"
                    },

                    AllowOfflineAccess = true,
                },
                new Client
                {
                    ClientId = "api.client.test",
                    ClientName = "Bruno Test Client",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret(clientSecret.Sha256()) },

                    AccessTokenLifetime = tokenLifetime,

                    AllowedScopes =
                    {
                        "MenuCatalog.api.full",
                        "DeliveryOrdering.api.full"
                    }
                }
            };
        }
    }
}
