using Newtonsoft.Json;

namespace LobAccelerator.Library.Models.SharePoint.Collections
{
    public class SiteCollection
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("owner")]
        public string Owner { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("template")]
        public string Template { get; set; }

        [JsonProperty("storageMaximumLevel")]
        public int StorageMaximumLevel { get; set; }

        [JsonProperty("userCodeMaximumLevel")]
        public int UserCodeMaximumLevel { get; set; }
    }
}
