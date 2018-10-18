using LobAccelerator.Library.Models.Teams.Channels;
using LobAccelerator.Library.Models.Teams.Groups;
using LobAccelerator.Library.Models.Teams.Teams;
using System.Collections.Generic;

namespace LobAccelerator.Library.Models.Teams.Results
{
    public class TeamResourceResults
    {
        public Group Group { get; set; }

        public Team Team { get; set; }

        public IEnumerable<Channel> Channels { get; set; }

        public IEnumerable<string> Members { get; set; }

        public IEnumerable<FileResult> Files { get; set; }
    }
}
