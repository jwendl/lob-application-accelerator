using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;

namespace LobAccelerator.Library.Models.Teams
{
    [JsonObject("channel")]
    public class ChannelResource
    {
        [JsonProperty("displayName")]
        [Description("The display name of the channel.")]
        public string DisplayName { get; set; }

        [JsonProperty("description")]
        [Description("The description of the channel.")]
        public string Description { get; set; }

        [JsonProperty("files")]
        [Description("The files to be copied from the user's OneDrive for Business to Teams channel.")]
        public IEnumerable<string> Files{ get; set; }

        [JsonProperty("members")]
        [Description("Channel Members, will resolve to https://graph.microsoft.com/beta/directoryObjects/<id>. For <id> use the AD Object name like me@contoso.com.")]
        public IEnumerable<string> Members { get; set; }

        [JsonProperty("sharepointList")]
        [Description("Sharepoint list to be linked with the channel thru a TAB.")]
        public string SharepointListUrl { get; set; }

        [JsonProperty("sharepointListName")]
        [Description("Sharepoint list name to show over the TAB")]
        public string SharepointListName { get; set; }

        public ChannelResource()
        {
            Files = new List<string>();
            Members = new List<string>();
        }
    }
}
