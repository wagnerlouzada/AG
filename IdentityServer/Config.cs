using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace Identity.IdentityServer
{
    public class Config
    {
        // scopes define the resources in your system
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("AgDataApi", "My API", new [] { "name" })
            };
        }

        // clients want to access resources (aka scopes)
        public static IEnumerable<Client> GetClients()
        {
            // client credentials client
            return new List<Client>
            {
                // OpenID Connect hybrid flow and client credentials client (MVC)
                new Client
                {
                    ClientId = "mvc",
                    ClientName = "MVC Client",
                    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,
                    RequireConsent = true,
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    RedirectUris = { "https://localhost:44340/signin-oidc" },
                    PostLogoutRedirectUris = { "https://localhost:44340/signout-callback-oidc" },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "AgDataApi"
                    },
                    AllowOfflineAccess = true
                },
                                new Client
                {
                    ClientId = "AgDataApi",
                    ClientName = "API Client",
                    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,
                    RequireConsent = true,
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    RedirectUris = { "https://localhost:62415/signin-oidc" },
                    PostLogoutRedirectUris = { "https://localhost:62415/signout-callback-oidc" },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "AgDataApi"
                    },
                    AllowOfflineAccess = true
                },
                new Client
                {
                    ClientId = "Spa",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "AgDataApi"
                    },
                    RedirectUris = { "https://localhost:44340/SignInCallback.html" },
                    PostLogoutRedirectUris = { "https://localhost:44340/SignOutCallback.html" },
                    AllowedCorsOrigins = { "https://localhost:44340" },
                    RequireConsent = false
                },
                // OpenID Connect implicit flow client (Angular)
                new Client
                {
                    ClientId = "ng",
                    ClientName = "Angular Client",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = true,

                    RedirectUris = { "https://localhost:44398/callback" },
                    PostLogoutRedirectUris = { "https://localhost:44398/home" },
                    AllowedCorsOrigins = { "https://localhost:44398" },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "AgDataApi"
                    },

                }
            };
        }
    }

}
