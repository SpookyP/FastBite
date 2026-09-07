using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Secret = Duende.IdentityServer.Models.Secret;

namespace FastBite.Identity.Api.Data
{
    /// <summary>
    /// Configurações do IdentityServer, como API scopes, recursos de API e clientes.
    /// </summary>
    public static class Config
    {
        /// <summary>
        /// Recursos de identidade disponíveis para o IdentityServer.
        /// </summary>
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),

                new IdentityResource("roles","Perfis de Utilizador", new[]{"role"})
            };
        /// <summary>
        /// Scopes de API com autorização disponíveis para o IdentityServer.
        /// </summary>
        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("MenuCatalog.api.full", "FastBite Food Catalog", new[]{"role"}),
                new ApiScope("DeliveryOrdering.api.full", "FastBite Food Ordering", new[]{"role"}),
            };

        /// <summary>
        /// Recursos de API disponíveis para o IdentityServer e mapeamento das respetivas scopes e claims.
        /// </summary>
        public static IEnumerable<ApiResource> ApiResources =>
            new ApiResource[]
            {
                new ApiResource("fastbite.menu", "FastBite Menu API")
                {
                    Scopes = { "MenuCatalog.api.full" },
                    UserClaims = { "role" }
                },
                new ApiResource("fastbite.order", "FastBite Order API")
                {
                    Scopes = { "DeliveryOrdering.api.full" },
                    UserClaims = { "role" }
                },
            };

        /// <summary>
        /// Clientes disponíveis para o IdentityServer, com respetivas configurações de autenticação e autorização.
        /// </summary>
        /// <param name="configuration"></param>Configuração usada para obter os valores de ClientOrigin e ClientSecret e token lifetimes.
        /// <returns>Collection of configured <see cref="Client"/> instances.</returns>
        public static IEnumerable<Client> GetClients(IConfiguration configuration)
        {
            var clientSecret = configuration["JwtSettings:Secret"] ?? throw new InvalidOperationException("Client secret is missing from configuration!");
            var redirectUris = configuration["JwtSettings:RedirectUri"] ?? throw new InvalidOperationException("RedirectUri is missing from configuration!");
            var postLogoutRedirectUris = configuration["JwtSettings:PostLogoutRedirectUri"] ?? throw new InvalidOperationException("PostLogoutRedirectUri is missing from configuration!");
            var clientOrigin = configuration["ClientOrigin"] ?? throw new InvalidOperationException("ClientOrigin is missing from configuration!");

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
                    AllowedCorsOrigins = { clientOrigin }, //Cross-Origin Resource Sharing (CORS)
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
