using LobAccelerator.Library.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LobAccelerator.Library.Managers
{
    public interface ITokenManager
    {
        Task<AuthenticationResult> GetOnBehalfOfAccessTokenAsync(string accessToken, IEnumerable<string> scopes);
    }

    public class TokenManager
        : ITokenManager
    {
        private readonly IConfiguration configuration;
        private readonly ITokenCacheHelper tokenCacheHelper;

        public TokenManager(IConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.tokenCacheHelper = new TokenCacheHelper(configuration);
        }

        public async Task<Uri> GetAuthUriAsync(IEnumerable<string> scopes)
        {
            try
            {
                var clientTokenCache = new TokenCache();
                var userTokenCache = tokenCacheHelper.FetchUserCache();
                var appTokenCache = new TokenCache();

                var msalApp = new ConfidentialClientApplication(
                    configuration["ClientId"],
                    configuration["RedirectUri"],
                    new ClientCredential(configuration["ClientSecret"]),
                        userTokenCache,
                        appTokenCache);

                var result = await msalApp.GetAuthorizationRequestUrlAsync(scopes,
                    null,
                    null);

                return result;
            }
            catch (MsalException mse)
            {
                throw mse;
            }
        }

        public async Task<AuthenticationResult> GetAccessTokenFromCodeAsync(string authCode, IEnumerable<string> scopes)
        {
            try
            {
                var clientTokenCache = new TokenCache();
                var userTokenCache = tokenCacheHelper.FetchUserCache();
                var appTokenCache = new TokenCache();

                var msalApp = new ConfidentialClientApplication(
                    configuration["ClientId"],
                    configuration["RedirectUri"],
                    new ClientCredential(configuration["ClientSecret"]),
                        userTokenCache,
                        appTokenCache);

                var result = await msalApp.AcquireTokenByAuthorizationCodeAsync(authCode, scopes);

                return result;
            }
            catch (MsalException mse)
            {
                throw mse;
            }
        }

        public async Task<AuthenticationResult> GetOnBehalfOfAccessTokenAsync(string accessToken, IEnumerable<string> scopes)
        {
            try
            {
                var clientTokenCache = new TokenCache();
                var userTokenCache = tokenCacheHelper.FetchUserCache();
                var appTokenCache = new TokenCache();

                var msalApp = new ConfidentialClientApplication(
                    configuration["ClientId"],
                    configuration["RedirectUri"],
                    new ClientCredential(configuration["ClientSecret"]),
                        userTokenCache,
                        appTokenCache);

                //var user = new UserAssertion(accessToken);
                var user = new UserAssertion(accessToken, "urn:ietf:params:oauth:grant-type:jwt-bearer");
                var result = await msalApp.AcquireTokenOnBehalfOfAsync(scopes,
                    user,
                    $"https://login.microsoftonline.com/{configuration["TenantId"]}");
                //var result = await msalApp.AcquireTokenOnBehalfOfAsync(scopes,
                //    user);

                return result;
            }
            catch (MsalException mse)
            {
                throw mse;
            }
        }
    }
}