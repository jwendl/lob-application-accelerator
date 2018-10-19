using Microsoft.Identity.Client;

namespace LobAccelerator.Library.Services.Interfaces
{
    public interface ITokenCacheService
    {
        TokenCache FetchUserCache();
    }
}
