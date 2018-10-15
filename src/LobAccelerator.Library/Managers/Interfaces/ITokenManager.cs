using Microsoft.Identity.Client;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LobAccelerator.Library.Managers.Interfaces
{
    public interface ITokenManager
    {
        Task<AuthenticationResult> GetOnBehalfOfAccessTokenAsync(string accessToken, IEnumerable<string> scopes);
    }
}
