using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace LobAccelerator.Library.Utils
{
    public class AuthHelper
    {
        static AuthHelper()
        {
        }

        /// <summary>
        /// Validate a bearer token, and make sure it matches expected issuer, audience
        /// </summary>
        /// <param name="authenticationHeaderValue"></param>
        /// <param name="expectedIssuer"></param>
        /// <param name="expectedAudience"></param>
        /// <param name="expectedScopes"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        public static async Task<ClaimsPrincipal> ValidateTokenAsync(AuthenticationHeaderValue authenticationHeaderValue, string expectedIssuer, string expectedAudience, string[] expectedScopes, ILogger log = null)
        {
            if (log == null)
                log = new DefaultLogger();


            if (authenticationHeaderValue?.Scheme != "Bearer")
            {
                throw new ArgumentException(string.Format("{0} is not supported", authenticationHeaderValue?.Scheme), "authenticationHeaderValue");
            }

            var documentRetriever = new HttpDocumentRetriever
            {
                RequireHttps = expectedIssuer.StartsWith("https://")
            };

            var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                $"https://login.microsoftonline.com/common/v2.0/.well-known/openid-configuration",
                new OpenIdConnectConfigurationRetriever(),
                documentRetriever
            );

            var configuration = await configurationManager.GetConfigurationAsync(CancellationToken.None);

            var validationParameter = new TokenValidationParameters()
            {
                RequireSignedTokens = true,
                ValidAudience = expectedAudience,
                ValidateAudience = true,
                ValidIssuer = expectedIssuer,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                IssuerSigningKeys = configuration.SigningKeys // need to figure out if we care about these
            };

            ClaimsPrincipal result = null;
            var tries = 0;

            while (result == null && tries <= 1)
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    result = handler.ValidateToken(authenticationHeaderValue.Parameter, validationParameter, out var token);
                    return result;
                }
                catch (SecurityTokenSignatureKeyNotFoundException)
                {
                    // This exception is thrown if the signature key of the JWT could not be found.
                    // This could be the case when the issuer changed its signing keys, so we trigger a 
                    // refresh and retry validation.
                    configurationManager.RequestRefresh();
                    tries++;
                }
                catch (SecurityTokenException ex)
                {
                    log.LogError(ex.Message);
                    return null;
                }
            }

            return null;
        }
    }
}
