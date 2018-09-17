using Newtonsoft.Json;
using System.ComponentModel;

namespace LobAccelerator.Library.Models.Teams
{
    [JsonObject("channel")]
    public class Channel
    {
        [JsonProperty("displayName")]
        [Description("The display name of the channel.")]
        public string DisplayName { get; set; }

        [JsonProperty("description")]
        [Description("The description of the channel.")]
        public string Description { get; set; }
    }
}
