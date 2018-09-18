using LobAccelerator.Library.Models.Common;
using System.Threading.Tasks;

namespace LobAccelerator.Library.Interfaces
{
    public interface IResourceManager<T>
    {
        Task CreateResourceAsync(T resource);
    }
}
