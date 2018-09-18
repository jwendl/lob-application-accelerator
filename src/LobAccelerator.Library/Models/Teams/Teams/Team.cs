using Newtonsoft.Json;
using System.ComponentModel;

namespace LobAccelerator.Library.Models.Teams.Teams
{
    public class Team
    {
        [JsonProperty("id")]
        [Description("Team ID")]
        public string Id { get; set; }
    }
}
