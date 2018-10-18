using LobAccelerator.Library.Interfaces;
using LobAccelerator.Library.Models.Teams;
using LobAccelerator.Library.Models.Teams.Results;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LobAccelerator.Library.Managers.Interfaces
{
    public interface ITeamsManager
        : IResourceManager<TeamResource, TeamResourceResults>
    {
        Task AddPeopleToChannelAsync(IEnumerable<string> members, string teamId);
        Task DeleteChannelAsync(string groupId);
    }
}
