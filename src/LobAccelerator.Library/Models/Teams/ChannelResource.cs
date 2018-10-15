using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;

namespace LobAccelerator.Library.Models.Teams
{
    public class ChannelResource
    {
        [JsonRequired]
        [DisplayName("Display Name")]
        [JsonProperty("displayName")]
        [Description("The display name of the channel.")]
        public string DisplayName { get; set; }

        [JsonRequired]
        [DisplayName("Description")]
        [JsonProperty("description")]
        [Description("The description of the channel.")]
        public string Description { get; set; }

        [DisplayName("Files")]
        [JsonProperty("files", NullValueHandling = NullValueHandling.Ignore)]
        [Description("The files to be copied from the user's OneDrive for Business to Teams channel.")]
        public IEnumerable<string> Files { get; set; }

        [DisplayName("Members")]
        [JsonProperty("members", NullValueHandling = NullValueHandling.Ignore)]
        [Description("Channel Members, will resolve to https://graph.microsoft.com/beta/directoryObjects/<id>. For <id> use the AD Object name like me@contoso.com.")]
        public IEnumerable<string> Members { get; set; }

        public ChannelResource()
        {
            Files = new List<string>();
            Members = new List<string>();
        }
    }
}
