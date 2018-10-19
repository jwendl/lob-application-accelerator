using LobAccelerator.Library.Interfaces;
using LobAccelerator.Library.Models.Users;
using System.Threading.Tasks;

namespace LobAccelerator.Library.Managers.Interfaces
{
    public interface IUserManager
        : IResourceManager<UserResource, UserResourceResult>
    {
        Task<UserBody> CreateUserAsync(UserResource userResource);
    }
}
