using LobAccelerator.Library.Extensions;
using LobAccelerator.Library.Interfaces;
using LobAccelerator.Library.Models;
using LobAccelerator.Library.Models.Teams;
using LobAccelerator.Library.Models.Teams.Groups;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LobAccelerator.Library.Managers
{
    public class TeamsManager : ITeamsManager
    {
        private readonly HttpClient httpClient;
        private readonly string _apiVersion;

        public TeamsManager(HttpClient httpClient)
        {
            this.httpClient = httpClient;
            _apiVersion = ConstantsExtension.TeamsApiVersion;
        }

        public Task CreateResourceAsync(Team resource)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new group where teams will be assigned to.
        /// </summary>
        /// <returns>Group HTTP Response</returns>
        public async Task CreateGroup(Team resource)
        {
            var groupUri = $"{_apiVersion}/groups";

            var requestContent = new GroupContent
            {
                Description = resource.Description,
                DisplayName = resource.DisplayName,
                GroupTypes = new string[] { GroupTypes.Unified.ToString() },
                MailEnabled = false,
                MailNickname = resource.MailNickname,
                SecurityEnabled = false,
                Members = resource.Members
            };

            await httpClient.PostContentAsync(groupUri, requestContent);
        }

        public async Task AddPeopleToChannelAsync(IEnumerable<string> members, string teamId)
        {
            var addMemberUrl = $"beta/groups/{teamId}/members/$ref";

            foreach (var member in members)
            {
                var channelObj = new CreateChannelGraphObject(member);
                await httpClient.PostContentAsync(addMemberUrl, channelObj);
            }
        }

        private class CreateChannelGraphObject
        {
            private readonly string memberId;

            [JsonProperty("@odata.id")]
            public string DisplayName
                => $"https://graph.microsoft.com/beta/directoryObjects/{memberId}";

            public CreateChannelGraphObject(string memberId)
            {
                this.memberId = memberId;
            }
        }
    }
}
