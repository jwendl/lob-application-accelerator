using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;

namespace LobAccelerator.Library.Models.Teams.Groups
{
    public class GroupBody
    {
        [DisplayName("Description")]
        [JsonProperty("description")]
        [Description("Group description")]
        public string Description { get; set; }

        [DisplayName("Display Name")]
        [JsonProperty("displayName")]
        [Description("The name to display in the address book for the group.")]
        public string DisplayName { get; set; }

        [DisplayName("Group Types")]
        [JsonProperty("groupTypes")]
        [Description("Office 365 or dynamic group.")]
        public List<string> GroupTypes { get; set; }

        [DisplayName("Mail Enabled")]
        [JsonProperty("mailEnabled")]
        [Description("Set this to true if creating an Office 365 Group or false if creating dynamic or security group.")]
        public bool MailEnabled { get; set; }

        [DisplayName("Mail Nickname")]
        [JsonProperty("mailNickname")]
        [Description("Set to true for security-enabled groups or if creating a dynamic or security group. False if creating an Office 365 Group.")]
        public string MailNickname { get; set; }

        [DisplayName("Security Enabled")]
        [JsonProperty("securityEnabled")]
        [Description("True for security-enabled groups or if creating a dynamic or security group. False if creating an Office 365 Group.")]
        public bool SecurityEnabled { get; set; }
    }
}
