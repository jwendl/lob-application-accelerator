using Newtonsoft.Json;
using System.ComponentModel;

namespace LobAccelerator.SchemaGenerator.Models
{
    [JsonObject("funSettings")]
    public class FunSettings
    {
        [JsonProperty("allowGiphy")]
        [Description("Whether or not giphy gifs can be used in the team.")]
        public bool AllowGiphy { get; set; }

        [JsonProperty("giphyContentRating")]
        [Description("What content rating to set for the giphy service.")]
        public string GiphyContentRating { get; set; }
    }
}
