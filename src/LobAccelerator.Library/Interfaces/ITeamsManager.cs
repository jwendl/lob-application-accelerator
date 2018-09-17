using LobAccelerator.Library.Models.Teams;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LobAccelerator.Library.Interfaces
{
    public interface ITeamsManager : IResourceManager<Team>
    {
        Task CreateResourceAsync(Team resource);
        Task AddPeopleToChannelAsync(IEnumerable<string> members, string teamId);
    }
}
