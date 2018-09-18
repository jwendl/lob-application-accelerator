using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;

namespace LobAccelerator.Library.Models.Teams.Groups
{
    [JsonObject("groupContent")]
    public class GroupContent
    {
        [JsonProperty("description")]
        [Description("Group description")]
        public string Description { get; set; }

        [JsonProperty("displayName")]
        [Description("The name to display in the address book for the group.")]
        public string DisplayName { get; set; }

        [JsonProperty("groupTypes")]
        [Description("Office 365 or dynamic group.")]
        public List<string> GroupTypes { get; set; }
        
        [JsonProperty("mailEnabled")]
        [Description("Set this to true if creating an Office 365 Group or false if creating dynamic or security group.")]
        public bool MailEnabled { get; set; }

        [JsonProperty("mailNickname")]
        [Description("Set to true for security-enabled groups or if creating a dynamic or security group. False if creating an Office 365 Group.")]
        public string MailNickname { get; set; }

        [JsonProperty("securityEnabled")]
        [Description("True for security-enabled groups or if creating a dynamic or security group. False if creating an Office 365 Group.")]
        public bool SecurityEnabled { get; set; }
    }
}
