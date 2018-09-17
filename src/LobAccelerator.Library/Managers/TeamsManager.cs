using LobAccelerator.Library.Extensions;
using LobAccelerator.Library.Interfaces;
using LobAccelerator.Library.Models;
using LobAccelerator.Library.Models.Teams;
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

        public TeamsManager(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task CreateResourceAsync(Team resource)
        {
            await AddPeopleToChannelAsync(resource.Members, resource.DisplayName);

            throw new NotImplementedException("Only AddPeopleToChannelAsync so far.");
        }

        public async Task AddPeopleToChannelAsync(IEnumerable<string> members, string teamId)
        {
            var addMemberUrl = $"beta/groups/{teamId}/members/$ref";

            foreach (var member in members)
            {
                var channelObj = new CreateChannelGraphObject(member);
                var response = await httpClient.PostContentAsync(addMemberUrl, channelObj);

                response.EnsureSuccessStatusCode();
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
