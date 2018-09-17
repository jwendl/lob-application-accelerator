using Newtonsoft.Json;
using System.ComponentModel;

namespace LobAccelerator.Library.Models
{
    [JsonObject("messageSettings")]
    public class MessagingSettings
    {
        [JsonProperty("allowUserEditMessages")]
        [Description("")]
        public bool AllowUserEditMessages { get; set; }

        [JsonProperty("allowUserDeleteMessages")]
        [Description("")]
        public bool AllowUserDeleteMessages { get; set; }
    }
}
