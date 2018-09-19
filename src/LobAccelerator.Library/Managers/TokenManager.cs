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
        Task<AuthenticationResult> GetAccessTokenAsync(AuthenticationHeaderValue authenticationHeaderValue, IEnumerable<string> scopes);
    }

    public class TokenManager
        : ITokenManager
    {
        private readonly IConfiguration configuration;
        private readonly ITokenCacheHelper tokenCacheHelper;

        public TokenManager(IConfiguration configuration, ITokenCacheHelper tokenCacheHelper)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.tokenCacheHelper = tokenCacheHelper ?? throw new ArgumentNullException(nameof(tokenCacheHelper));
        }

        public async Task<AuthenticationResult> GetAccessTokenAsync(AuthenticationHeaderValue authenticationHeaderValue, IEnumerable<string> scopes)
        {
            var clientTokenCache = new TokenCache();
            var userTokenCache = tokenCacheHelper.FetchUserCache();
            var appTokenCache = new TokenCache();

            var msalApp = new ConfidentialClientApplication(
                configuration["ApplicationId"],
                configuration["RedirectUri"],
                new ClientCredential(configuration["ApplicationSecret"]),
                    userTokenCache,
                    appTokenCache);

            var user = new UserAssertion(authenticationHeaderValue.Parameter);

            var result = await msalApp.AcquireTokenOnBehalfOfAsync(scopes,
                user,
                configuration["Authority"]);

            return result;
        }
    }
}