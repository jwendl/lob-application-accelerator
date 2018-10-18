using System.Threading.Tasks;

namespace LobAccelerator.Library.Interfaces
{
    public interface IResourceManager<TInput, TResult>
    {
        Task<TResult> CreateResourceAsync(TInput resource);
    }
}
