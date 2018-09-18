using Newtonsoft.Json;
using System.ComponentModel;

namespace LobAccelerator.Library.Models.Teams.Groups
{
    public class Group
    {
        [JsonProperty("id")]
        [Description("Group ID")]
        public string Id { get; set; }
    }
}
