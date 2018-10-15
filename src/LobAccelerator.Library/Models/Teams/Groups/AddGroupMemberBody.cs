using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LobAccelerator.Library.Models.Teams.Groups
{
    public class AddGroupMemberBody
    {
        private readonly string upn;

        [JsonProperty("@odata.id")]
        public string IdReference => $"https://graph.microsoft.com/beta/users/{upn}";

        public AddGroupMemberBody(string upn)
        {
            this.upn = upn;
        }
    }
}
