using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Identity.Core.Cache;
using System.Net.Http;

namespace LobAccelerator.Library.Managers
{
    class TokenManager
    {
        static async Task<AuthenticationResult> GetAccessTokenAsync(IEnumerable<string> scopes)
        {
            // NO BUENO, NO REFRESH TOKEN!!!
            /*var clientTokenCache = new TokenCache();
            var userTokenCache = new TokenCache();
            var appTokenCache = new TokenCache();

            var msalApp = new ConfidentialClientApplication(
                System.Configuration.ConfigurationManager.AppSettings["ApplicationId"],
                System.Configuration.ConfigurationManager.AppSettings["Authority"],
                System.Configuration.ConfigurationManager.AppSettings["RedirectUri"],
                new ClientCredential(System.Configuration.ConfigurationManager.AppSettings["ApplicationSecret"]),
                userTokenCache,
                appTokenCache);

            var user = new UserAssertion(authenticationHeaderValue.Parameter);

            return await msalApp.AcquireTokenOnBehalfOfAsync(scopes, user, System.Configuration.ConfigurationManager.AppSettings["Authority"]);*/

            // Need assertion param?
            var client = new HttpClient();
            var resp = await client.PostAsync(
                new Uri(string.Format("https://login.microsoftonline.com/{0}/oauth2/v2.0/token",
                    System.Configuration.ConfigurationManager.AppSettings["TenantId"])),
                new StringContent(string.Format("grant_type=urn:ietf:params:oauth:grant-type:jwt-bearer" +
                    "&client_id={0}" +
                    "&client_secret={1}" +
                    "&scope=https://graph.microsoft.com/user.read+offline_access" +
                    "&requested_token_use= on_behalf_of",
                    System.Configuration.ConfigurationManager.AppSettings["ApplicationId"],
                    System.Configuration.ConfigurationManager.AppSettings["ApplicationSecret"]))
            );

            return null;
        }

        static async Task<string> RefreshTokenAsync(string refreshToken)
        {
            return null;
        }
    }
}
