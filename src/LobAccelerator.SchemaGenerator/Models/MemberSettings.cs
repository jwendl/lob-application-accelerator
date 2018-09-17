using Newtonsoft.Json;

namespace LobAccelerator.SchemaGenerator.Models
{
    [JsonObject("memberSettings")]
    public class MemberSettings
    {
        [JsonProperty("allowCreateUpdateChannels")]
        public bool AllowCreateUpdateChannels { get; set; }
    }
}
