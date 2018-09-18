using LobAccelerator.Library.Models.Common;
using LobAccelerator.Library.Models.Teams;
using LobAccelerator.Library.Models.Teams.Groups;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LobAccelerator.Library.Interfaces
{
    public interface ITeamsManager : IResourceManager<Team>
    {
        Task<IResult> CreateResourceAsync(Team resource);
        Task<Result<Group>> CreateGroupAsync(Team resource);
        Task AddPeopleToChannelAsync(IEnumerable<string> members, string teamId);
    }
}
