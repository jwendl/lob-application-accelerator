using LobAccelerator.Library.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace LobAccelerator.Library.Managers
{
    public interface ITokenManager
    {
        Task<AuthenticationResult> GetAccessTokenAsync(string accessToken, IEnumerable<string> scopes);
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

        public async Task<AuthenticationResult> GetAccessTokenAsync(string accessToken, IEnumerable<string> scopes)
        {
            try { 
                var clientTokenCache = new TokenCache();
                var userTokenCache = tokenCacheHelper.FetchUserCache();
                var appTokenCache = new TokenCache();

                var msalApp = new ConfidentialClientApplication(
                    configuration["ApplicationId"],
                    configuration["RedirectUri"],
                    new ClientCredential(configuration["ApplicationSecret"]),
                        userTokenCache,
                        appTokenCache);

                msalApp.ValidateAuthority = false;

                var user = new UserAssertion(accessToken);
                var result = await msalApp.AcquireTokenOnBehalfOfAsync(scopes,
                    user,
                    configuration["Authority"]);

                return result;
            }
            catch (MsalException mse)
            {
                throw mse;
            }
        }
    }
}