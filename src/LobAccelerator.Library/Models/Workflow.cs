using LobAccelerator.Library.Models.Azure;
using LobAccelerator.Library.Models.Teams;
using LobAccelerator.Library.Models.Users;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;

namespace LobAccelerator.Library.Models
{
    public class Workflow
    {
        public Workflow()
        {
            Teams = new List<TeamResource>();
            Users = new List<UserResource>();
            ARMDeployments = new List<ARMDeployment>();
        }

        [DisplayName("Teams")]
        [JsonProperty("teams", NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<TeamResource> Teams { get; set; }

        [DisplayName("Users")]
        [JsonProperty("users", NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<UserResource> Users { get; set; }

        [DisplayName("ARM Deployments")]
        [JsonProperty("armDeployments", NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<ARMDeployment> ARMDeployments { get; set; }
    }
}
