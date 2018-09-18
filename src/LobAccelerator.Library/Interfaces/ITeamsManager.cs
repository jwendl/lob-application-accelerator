﻿using LobAccelerator.Library.Models.Teams;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LobAccelerator.Library.Interfaces
{
    public interface ITeamsManager : IResourceManager<TeamResource>
    {
        Task<IResult> CreateResourceAsync(TeamResource resource);
        Task<IResult> AddPeopleToChannelAsync(IEnumerable<string> members, string teamId);
    }
}
