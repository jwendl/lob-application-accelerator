using LobAccelerator.Library.Extensions;
using LobAccelerator.Library.Interfaces;
using LobAccelerator.Library.Models;
using LobAccelerator.Library.Models.Common;
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

        public async Task<Result> CreateResourceAsync(Team resource)
        {
            var groupResult = await CreateGroupAsync(resource);

            if (groupResult.HasError)
            {

            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new group where teams will be assigned to.
        /// </summary>
        /// <returns>Group ID</returns>
        public async Task<Result> CreateGroupAsync(Team resource)
        {
            var result = new Result();
            var groupUri = $"{_apiVersion}/groups";

            var requestContent = new GroupContent
            {
                Description = resource.Description,
                DisplayName = resource.DisplayName,
                GroupTypes = new List<string> { GroupTypes.Unified.ToString() },
                MailEnabled = true,
                MailNickname = resource.MailNickname,
                SecurityEnabled = false
            };

            var response = await httpClient.PostContentAsync(groupUri, requestContent);

            if (!response.IsSuccessStatusCode)
            {
                result.HasError = true;
                result.ErrorMessage = response.ReasonPhrase;

                return result;
            }

            var groupIdResult = await GetGroupIdByDisplayName(resource.DisplayName);

            if (groupIdResult.HasError)
            {
                result.HasError = true;
                result.ErrorMessage = groupIdResult.ErrorMessage;

                return result;
            }

            result.Value = groupIdResult.Value;

            return result;
        }

        /// <summary>
        /// Returns the group Id based on the display name.
        /// </summary>
        /// <param name="displayName"></param>
        /// <returns></returns>
        public async Task<Result> GetGroupIdByDisplayName(string displayName)
        {
            var result = new Result();
            var uri = $"{_apiVersion}/groups?$filter=displayName eq '{displayName}'";

            var response = await httpClient.GetContentAsync(uri);

            if (!response.IsSuccessStatusCode)
            {
                result.HasError = true;
                result.ErrorMessage = response.ReasonPhrase;
            }

            result.Value = await response.Content.ReadAsStringAsync();

            return result;
        }

        /// <summary>
        /// Creates a new Teams
        /// </summary>
        /// <returns></returns>
        public async Task CreateTeam()
        {

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
