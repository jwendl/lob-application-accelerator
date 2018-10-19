using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;

namespace LobAccelerator.Library.Models.Teams
{
    public class TeamResource
    {
        public TeamResource()
        {
            MemberSettings = new MemberSettings()
            {
                AllowCreateUpdateChannels = true,
            };

            MessagingSettings = new MessagingSettings()
            {
                AllowUserEditMessages = true,
                AllowUserDeleteMessages = false
            };

            FunSettings = new FunSettings()
            {
                AllowGiphy = true,
                GiphyContentRating = "strict",
            };
        }

        [DisplayName("Clone From Id")]
        [JsonProperty("cloneFromId", NullValueHandling = NullValueHandling.Ignore)]
        [Description("The id of the team's page to clone from.")]
        public string CloneFromId { get; set; }

        [JsonRequired]
        [DisplayName("Display Name")]
        [JsonProperty("displayName")]
        [Description("The name to display in the address book for the group.")]
        public string DisplayName { get; set; }

        [JsonRequired]
        [DisplayName("Description")]
        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        [Description("The description of the teams group.")]
        public string Description { get; set; }

        [DisplayName("Mail Nickname")]
        [JsonProperty("mailNickname", NullValueHandling = NullValueHandling.Ignore)]
        [Description("The mail alias for the team.")]
        public string MailNickname { get; set; }

        [DisplayName("Member Settings")]
        [JsonProperty("memberSettings", NullValueHandling = NullValueHandling.Ignore)]
        [Description("Member settings for creating a team.")]
        public MemberSettings MemberSettings { get; set; }

        [DisplayName("Messaging Settings")]
        [JsonProperty("messagingSettings", NullValueHandling = NullValueHandling.Ignore)]
        [Description("Messaging settings for creating a team.")]
        public MessagingSettings MessagingSettings { get; set; }

        [DisplayName("Fun Settings")]
        [JsonProperty("funSettings", NullValueHandling = NullValueHandling.Ignore)]
        [Description("Fun settings for giphy and stuff.")]
        public FunSettings FunSettings { get; set; }

        [JsonRequired]
        [DisplayName("Members")]
        [JsonProperty("members")]
        [Description("Group Members, will resolve to https://graph.microsoft.com/beta/directoryObjects/<id>. For <id> use the AD Object name like me@contoso.com.")]
        public IEnumerable<string> Members { get; set; }

        [JsonRequired]
        [DisplayName("Channels")]
        [JsonProperty("channels")]
        [Description("Channels to add to this team.")]
        public IEnumerable<ChannelResource> Channels { get; set; }
    }
}
