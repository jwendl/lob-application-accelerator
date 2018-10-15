using LobAccelerator.Library.Interfaces;
using LobAccelerator.Library.Models.Common;
using LobAccelerator.Library.Models.Users;
using System.Threading.Tasks;

namespace LobAccelerator.Library.Managers.Interfaces
{
    public interface IUserManager
        : IResourceManager<UserResource>
    {
        Task<Result<UserBody>> CreateUserAsync(UserResource userResource);
    }
}
