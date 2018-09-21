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
        Task<AuthenticationResult> GetOnBehalfOfAccessTokenAsync(IEnumerable<string> scopes, string accessToken = null);
        void UpdateAccessToken(string accessToken);
    }

    public class TokenManager
        : ITokenManager
    {
        private readonly IConfiguration configuration;
        private readonly ITokenCacheHelper tokenCacheHelper;
        private string _accessToken;

        public TokenManager(IConfiguration configuration, string accessToken = null)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this._accessToken = accessToken;
            this.tokenCacheHelper = new TokenCacheHelper(configuration);
        }

        public void UpdateAccessToken(string accessToken)
        {
            this._accessToken = accessToken;
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

        public async Task<AuthenticationResult> GetOnBehalfOfAccessTokenAsync(IEnumerable<string> scopes, string accessToken = null)
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

                var user = new UserAssertion(accessToken ?? _accessToken,
                    "urn:ietf:params:oauth:grant-type:jwt-bearer");
                var result = await msalApp.AcquireTokenOnBehalfOfAsync(scopes,
                    user,
                    $"https://login.microsoftonline.com/{configuration["TenantId"]}");

                return result;
            }
            catch (MsalException mse)
            {
                throw mse;
            }
        }
    }
}