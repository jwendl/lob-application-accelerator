using Newtonsoft.Json;

namespace LobAccelerator.Library.Models
{
    [JsonObject("memberSettings")]
    public class MemberSettings
    {
        [JsonProperty("allowCreateUpdateChannels")]
        public bool AllowCreateUpdateChannels { get; set; }
    }
}
