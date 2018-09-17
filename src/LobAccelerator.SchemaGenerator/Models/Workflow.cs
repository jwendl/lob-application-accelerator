using Newtonsoft.Json;
using System.Collections.Generic;

namespace LobAccelerator.SchemaGenerator.Models
{
    [JsonObject("workflow")]
    public class Workflow
    {
        [JsonProperty("teams")]
        public IEnumerable<Team> Teams { get; set; }
    }
}
