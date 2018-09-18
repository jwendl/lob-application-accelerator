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

namespace LobAccelerator.Library.Managers
{
    class TokenManager
    {
        static async Task<string> RefreshTokenAsync(string refreshToken)
        {
            return null;
        }
    }
}
