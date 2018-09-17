using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;

namespace LobAccelerator.SchemaGenerator.Models
{
    [JsonObject("team")]
    public class Team
    {
        [JsonProperty("cloneFromId", NullValueHandling = NullValueHandling.Ignore)]
        [Description("The id of the team's page to clone from.")]
        public string CloneFromId { get; set; }

        [JsonProperty("displayName")]
        [Description("The name to display in the address book for the group.")]
        public string DisplayName { get; set; }

        [JsonProperty("description")]
        [Description("The description of the teams group.")]
        public string Description { get; set; }

        [JsonProperty("mailNickname")]
        [Description("The mail alias for the team.")]
        public string MailNickname { get; set; }

        [JsonProperty("memberSettings")]
        [Description("Member settings for creating a team.")]
        public MemberSettings MemberSettings { get; set; }

        [JsonProperty("messagingSettings")]
        [Description("Messaging settings for creating a team.")]
        public MessagingSettings MessagingSettings { get; set; }

        [JsonProperty("funSettings")]
        [Description("Fun settings for giphy and stuff.")]
        public FunSettings FunSettings { get; set; }

        [JsonProperty("members")]
        [Description("Group Members, will resolve to https://graph.microsoft.com/beta/directoryObjects/<id>. For <id> use the AD Object name like me@contoso.com.")]
        public IEnumerable<string> Members { get; set; }

        [JsonProperty("channels")]
        [Description("Channels to add to this team.")]
        public IEnumerable<Channel> Channels { get; set; }
    }
}
