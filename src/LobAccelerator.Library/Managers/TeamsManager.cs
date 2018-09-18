using LobAccelerator.Library.Extensions;
using LobAccelerator.Library.Interfaces;
using LobAccelerator.Library.Models.Common;
using LobAccelerator.Library.Models.Teams;
using LobAccelerator.Library.Models.Teams.Channels;
using LobAccelerator.Library.Models.Teams.Groups;
using LobAccelerator.Library.Models.Teams.Teams;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace LobAccelerator.Library.Managers
{
    public class TeamsManager : ITeamsManager
    {
        private readonly HttpClient httpClient;
        private readonly string _apiVersion;
        private HttpResponseMessage responseDeletePerm;

        public TeamsManager(HttpClient httpClient)
        {
            this.httpClient = httpClient;
            _apiVersion = ConstantsExtension.TeamsApiVersion;
        }

        public async Task<IResult> CreateResourceAsync(TeamResource resource)
        {
            Result<Group> group = await CreateGroupAsync(resource);
            Result<Team> team = await CreateTeamAsync(group.Value.Id, resource);
            IResult channels = await CreateChannelsAsync(team.Value.Id, resource.Channels);
            
            return Result.Combine(group, team, channels);
        }

        /// <summary>
        /// Creates a new group where teams will be assigned to.
        /// </summary>
        /// <returns>Group ID</returns>
        public async Task<Result<Group>> CreateGroupAsync(TeamResource resource)
        {
            var result = new Result<Group>();
            var groupUri = $"{_apiVersion}/groups";

            var requestContent = new GroupBody
            {
                Description = resource.Description,
                DisplayName = resource.DisplayName,
                GroupTypes = new List<string> { GroupTypes.Unified.ToString() },
                MailEnabled = true,
                MailNickname = resource.MailNickname,
                SecurityEnabled = false
            };

            var response = await httpClient.PostContentAsync(groupUri, requestContent);
            var responseString = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                result.Value = JsonConvert.DeserializeObject<Group>(responseString);
                return result;
            }

            result.HasError = true;
            result.Error = response.ReasonPhrase;
            result.DetailedError = responseString;

            return result;
        }

        /// <summary>
        /// Creates a new Team to an existing group.
        /// </summary>
        /// <returns></returns>
        public async Task<Result<Team>> CreateTeamAsync(string groupId, TeamResource resource)
        {
            var result = new Result<Team>();
            var uri = $"{_apiVersion}/groups/{groupId}/team";

            var requestContent = new TeamBody
            {
                MemberSettings = resource.MemberSettings,
                MessagingSettings = resource.MessagingSettings,
                FunSettings = resource.FunSettings
            };

            var response = await httpClient.PutContentAsync(uri, requestContent);
            var responseString = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                result.Value = JsonConvert.DeserializeObject<Team>(responseString);
                return result;
            }

            result.HasError = true;
            result.Error = response.ReasonPhrase;
            result.DetailedError = responseString;

            return result;
        }

        /// <summary>
        /// Create channels under a team.
        /// </summary>
        /// <param name="teamsId">Team ID</param>
        /// <param name="channels">List of channels to be added</param>
        /// <returns></returns>
        public async Task<IResult> CreateChannelsAsync(string teamId, IEnumerable<ChannelResource> channels)
        {
            var results = new List<Result<Channel>>();
            var uri = $"{_apiVersion}/teams/{teamId}/channels";

            foreach(var channel in channels)
            {
                var result = new Result<Channel>();
                var response = await httpClient.PostContentAsync(uri, channel);
                var responseString = await response.Content.ReadAsStringAsync();
                
                if (response.IsSuccessStatusCode)
                {
                    result.Value = JsonConvert.DeserializeObject<Channel>(responseString);
                }
                else
                {
                    result.HasError = true;
                    result.Error = response.ReasonPhrase;
                    result.DetailedError = responseString;
                }

                results.Add(result);
            }

            return Result.Combine(results);
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

        public async Task<string> SearchTeamAsync(string displayName)
        {
            var listTeams = $"beta/groups?$filter=displayName eq '{displayName}'&$select=id";

            var response = await httpClient.GetAsync(listTeams);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var contentObj = JObject.Parse(content);

            return contentObj["value"][0]["id"].Value<string>();
        }

        public async Task<Result<NoneResult>> DeleteChannelAsync(string groupId)
        {
            var result = new Result<NoneResult>();
            var deleteUri = $"{_apiVersion}/groups/{groupId}";
            var deletePermanentUri = $"{_apiVersion}/directory/deleteditems/microsoft.graph.group/{groupId}";

            var responseDelete = await httpClient.DeleteAsync(deleteUri);
            responseDelete.EnsureSuccessStatusCode();

            var responseDeletePerm = await httpClient.DeleteAsync(deletePermanentUri);
            responseDeletePerm.EnsureSuccessStatusCode();

            return result;
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
