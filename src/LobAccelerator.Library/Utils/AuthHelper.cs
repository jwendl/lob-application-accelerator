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
    class AuthHelper
    {
        static AuthHelper()
        {
        }

        /// <summary>
        /// Validate a bearer token, and make sure it matches expected issuer, audience, scopes
        /// </summary>
        /// <param name="authenticationHeaderValue"></param>
        /// <param name="expectedIssuer"></param>
        /// <param name="expectedAudience"></param>
        /// <param name="expectedScopes"></param>
        /// <returns></returns>
        static bool ValidateToken(AuthenticationHeaderValue authenticationHeaderValue, string expectedIssuer, string expectedAudience, string[] expectedScopes)
        {
            // 1) token is valid syntactically

            if (authenticationHeaderValue?.Scheme != "Bearer")
            {
                return false;
            }

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

            return result;
        }

        // 2) token's issuer matches expected

        // 3) token's audience matches expected

        // 4) token's scopes match expected (get them from cycling through the claims)
    }
    }
}
