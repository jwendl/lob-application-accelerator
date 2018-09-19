using Newtonsoft.Json;

namespace SharepointConsoleApp.Models.SharePoint
{
    public class SiteCollection
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
