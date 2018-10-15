using LobAccelerator.Library.Interfaces;
using LobAccelerator.Library.Models.Common;
using LobAccelerator.Library.Models.Teams;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LobAccelerator.Library.Managers.Interfaces
{
    public interface ITeamsManager
        : IResourceManager<TeamResource>
    {
        Task<IResult> AddPeopleToChannelAsync(IEnumerable<string> members, string teamId);
        Task<Result<NoneResult>> DeleteChannelAsync(string groupId);
    }
}
