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

        [JsonProperty("azFilesFolderPath")]
        [Description("A path on azure sotrage account to get files to be uploaded and linked to this channel")]
        public string AzFilesFolderPath{ get; set; }

        [JsonProperty("members")]
        [Description("Channel Members, will resolve to https://graph.microsoft.com/beta/directoryObjects/<id>. For <id> use the AD Object name like me@contoso.com.")]
        public IEnumerable<string> Members { get; set; }
    }
}
