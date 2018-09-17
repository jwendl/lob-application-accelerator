using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LobAccelerator.Library.Interfaces
{
    public interface IResourceManager<T>
    {
        Task CreateResourceAsync(T resource);
    }
}
