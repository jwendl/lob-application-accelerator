using LobAccelerator.Library.Models.Teams;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace LobAccelerator.Library.Models
{
    [JsonObject("workflow")]
    public class Workflow
    {
        [JsonProperty("teams")]
        public IEnumerable<Team> Teams { get; set; }
    }
}
