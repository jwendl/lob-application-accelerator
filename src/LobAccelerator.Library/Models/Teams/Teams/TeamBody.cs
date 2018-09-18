using Newtonsoft.Json;

namespace LobAccelerator.Library.Models.Teams.Teams
{
    public class TeamBody
    {
        [JsonProperty("memberSettings")]
        public MemberSettings MemberSettings { get; set; }

        [JsonProperty("messagingSettings")]
        public MessagingSettings MessagingSettings { get; set; }

        [JsonProperty("funSettings")]
        public FunSettings FunSettings { get; set; }
    }
}
