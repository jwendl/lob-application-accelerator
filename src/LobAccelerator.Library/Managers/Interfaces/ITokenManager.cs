using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LobAccelerator.Library.Managers.Interfaces
{
    public interface ITokenManager
    {
        Task<Uri> GetAuthUriAsync(IEnumerable<string> scopes);
        Task<AuthenticationResult> GetAccessTokenFromCodeAsync(string authCode, IEnumerable<string> scopes);
        Task<AuthenticationResult> GetOnBehalfOfAccessTokenAsync(string accessToken, IEnumerable<string> scopes);
    }
}
