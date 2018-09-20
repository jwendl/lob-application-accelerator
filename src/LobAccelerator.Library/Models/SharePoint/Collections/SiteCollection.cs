using Newtonsoft.Json;

namespace LobAccelerator.Library.Models.SharePoint.Collections
{
    public class SiteCollection
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
