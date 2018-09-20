using Newtonsoft.Json;
using System.ComponentModel;

namespace LobAccelerator.Library.Models.SharePoint
{
    [JsonObject("sharePoint")]
    public class SharePointResource
    {
        [JsonProperty("displayName")]
        [Description("The name to display in the address book for the group.")]
        public string DisplayName { get; set; }
    }
}
