using LobAccelerator.Library.Managers.Interfaces;
using LobAccelerator.Library.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LobAccelerator.Library.Managers
{
    public class TokenManager
        : ITokenManager
    {
        private readonly IConfiguration configuration;
        private readonly ILogger logger;
        private readonly ITokenCacheService tokenCacheService;

        public TokenManager(IConfiguration configuration, ITokenCacheService tokenCacheService, ILogger logger)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.tokenCacheService = tokenCacheService;
            this.logger = logger;
        }

        /// <summary>
        /// Given a set of scopes, uses MSAL to formulate the proper Uri for the first part of the Code auth flow.
        /// See https://docs.microsoft.com/en-us/azure/active-directory/develop/v2-oauth2-auth-code-flow for more details on the flow.
        /// </summary>
        /// <param name="scopes">List of desired scopes for the Access Token.  For the Worker classes in this library, 
        /// this should be set to <code>$api://{configurationManager["ClientId"]}/access_as_user</code></param>
        /// <returns>Auth Uri that should be requested via GET to start the Code auth flow</returns>
        /// <exception cref="Microsoft.Identity.Client.MsalException">Throws an MsalException if there are any authentication issues</exception>
        public async Task<Uri> GetAuthUriAsync(IEnumerable<string> scopes)
        {
            try
            {
                logger.LogInformation("Getting Auth Request Uri using scopes: [{0}]...", string.Join(", ", scopes));

                var clientTokenCache = new TokenCache();
                var userTokenCache = tokenCacheService.FetchUserCache();
                var appTokenCache = new TokenCache();

                var msalApp = new ConfidentialClientApplication(configuration["ClientId"], configuration["RedirectUri"], new ClientCredential(configuration["ClientSecret"]), userTokenCache, appTokenCache);
                var result = await msalApp.GetAuthorizationRequestUrlAsync(scopes, null, null);

                return result;
            }
            catch (MsalException msalException)
            {
                logger.LogError(new EventId(0), msalException, $"Exception getting Auth Request Uri using scopes: [string.Join(", ", scopes)]. Look at the app registration and make sure you have the correct ClientId, ClientSecret and RedirectUri configured in the app settings...");
                throw msalException;
            }
        }

        /// <summary>
        /// Given an Auth Code and set of desired scopes, uses MSAL to get an Access token for the second part of the Code auth flow.
        /// See https://docs.microsoft.com/en-us/azure/active-directory/develop/v2-oauth2-auth-code-flow for more details on the flow.
        /// </summary>
        /// <param name="authCode">An Auth Code generated in the first step of the Code auth flow</param>
        /// <param name="scopes">List of desired scopes for the Access Token.  Should match the scopes used to get the auth Code.  
        /// For the Worker classes in this library, this should be set to <code>$api://{configurationManager["ClientId"]}/access_as_user</code></param>
        /// <returns>The AuthenticationResult with the resultant Access Token if the Code auth was successful</returns>
        /// <exception cref="Microsoft.Identity.Client.MsalException">Throws an MsalException if there are any authentication issues</exception>
        public async Task<AuthenticationResult> GetAccessTokenFromCodeAsync(string authCode, IEnumerable<string> scopes)
        {
            try
            {
                logger.LogInformation("Getting Access Token using Auth Code: {0}...", authCode);

                var clientTokenCache = new TokenCache();
                var userTokenCache = tokenCacheService.FetchUserCache();
                var appTokenCache = new TokenCache();

                var msalApplication = new ConfidentialClientApplication(configuration["ClientId"], configuration["RedirectUri"], new ClientCredential(configuration["ClientSecret"]), userTokenCache, appTokenCache);
                var result = await msalApplication.AcquireTokenByAuthorizationCodeAsync(authCode, scopes);

                return result;
            }
            catch (MsalException msalException)
            {
                logger.LogError(new EventId(0), msalException, $"Exception Getting Access Token using Auth Code: {authCode}. Look at the app registration and make sure you have the correct ClientId, ClientSecret and RedirectUri configured in the app settings...");
                throw msalException;
            }
        }

        /// <summary>
        /// Given a valid AccessToken and set of desired scopes and an Access Token, gets a new on-behalf-of Access Token.
        /// </summary>
        /// <param name="accessToken">A valid Access Token</param>
        /// <param name="scopes">List of desired scopes for the Access Token.</param>
        /// <returns>The AuthenticationResult with the resultant Access Token if the on-behalf-of auth was successful</returns>
        /// <exception cref="Microsoft.Identity.Client.MsalException">Throws an MsalException if there are any authentication issues</exception>
        public async Task<AuthenticationResult> GetOnBehalfOfAccessTokenAsync(string accessToken, IEnumerable<string> scopes)
        {
            try
            {
                logger.LogInformation("Getting On-Behalf-of Access Token using accessToken: <EXCLUDED FOR SECURITY>...");

                var clientTokenCache = new TokenCache();
                var userTokenCache = tokenCacheService.FetchUserCache();
                var appTokenCache = new TokenCache();

                var msalApplication = new ConfidentialClientApplication(configuration["ClientId"], configuration["RedirectUri"], new ClientCredential(configuration["ClientSecret"]), userTokenCache, appTokenCache);
                var user = new UserAssertion(accessToken, "urn:ietf:params:oauth:grant-type:jwt-bearer");
                var result = await msalApplication.AcquireTokenOnBehalfOfAsync(scopes, user, $"https://login.microsoftonline.com/{configuration["TenantId"]}");

                return result;
            }
            catch (MsalException msalException)
            {
                logger.LogError(new EventId(0), msalException, $"Exception On-Behalf-of Access Token using accessToken: <EXCLUDED FOR SECURITY>. Look at the app registration and make sure you have the correct ClientId, ClientSecret and RedirectUri configured in the app settings...");
                throw msalException;
            }
        }
    }
}