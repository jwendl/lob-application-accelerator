using LobAccelerator.Library.Extensions;
using LobAccelerator.Library.Interfaces;
using LobAccelerator.Library.Models.Common;
using LobAccelerator.Library.Models.Teams;
using LobAccelerator.Library.Models.Teams.Channels;
using LobAccelerator.Library.Models.Teams.Groups;
using LobAccelerator.Library.Models.Teams.Members;
using LobAccelerator.Library.Models.Teams.Teams;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LobAccelerator.Library.Managers
{
    public class TeamsManager
        : ITeamsManager
    {
        private readonly HttpClient httpClient;
        private readonly string _apiVersion;
        private readonly HttpResponseMessage responseDeletePerm;
        private readonly IOneDriveManager oneDriveManager;

        public TeamsManager(HttpClient httpClient, IOneDriveManager oneDriveManager)
        {
            this.httpClient = httpClient;
            _apiVersion = ConstantsExtension.TeamsApiVersion;

            this.oneDriveManager = oneDriveManager;
        }

        public async Task<IResult> CreateResourceAsync(TeamResource resource)
        {
            Result<Group> group = await CreateGroupAsync(resource);
            Result<Team> team = await CreateTeamAsync(group.Value.Id, resource);
            IResult channels = await CreateChannelsAsync(team.Value.Id, resource.Channels);
            IResult members = await AddPeopleToChannelAsync(resource.Members, team.Value.Id);
            IResult files = await CopyFilesToChannels(resource.Channels, team.Value.Id);

            return Result.Combine(group, team, channels, members, files);
        }

        private async Task<IResult> CopyFilesToChannels(IEnumerable<ChannelResource> channels, string teamId)
        {
            await Task.Delay(16000);
            // TODO: Remove this call.
            // BUG: Creating Teams through Graph is taking too long to propagate the files directory properties.

            var results = new List<Result<NoneResult>>();

            foreach (var channel in channels)
            {
                foreach (var resource in channel.Files)
                {
                    var result = new Result<NoneResult>();
                    try
                    {
                        if (IsFile(resource))
                            await oneDriveManager.CopyFileFromOneDriveToTeams(teamId, resource);
                        else
                            await oneDriveManager.CopyFolderFromOneDriveToTeams(teamId, resource);
                    }
                    catch (Exception ex)
                    {
                        result.HasError = true;
                        result.Error = ex.Message;
                    }
                    results.Add(result);
                }
            }

            return Result.Combine(results);
        }

        private bool IsFile(string input)
            => new System.Text.RegularExpressions.Regex(@"\.[a-zA-Z0-9]*$").IsMatch(input);

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

            foreach (var channel in channels)
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

        public async Task<IResult> AddPeopleToChannelAsync(IEnumerable<string> members, string teamId)
        {
            var results = new List<Result<NoneResult>>();
            var addMemberUrl = $"{_apiVersion}/groups/{teamId}/members/$ref";

            foreach (var member in members)
            {

                var result = new Result<NoneResult>();
                try
                {
                    var user = await GetUserAsync(member);
                    var channelObj = new CreateChannelGraphObject(user.Value);
                    var response = await httpClient.PostContentAsync(addMemberUrl, channelObj);
                    var responseString = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        result.HasError = true;
                        result.Error = response.ReasonPhrase;
                        result.DetailedError = responseString;
                    }
                }
                catch (Exception ex)
                {
                    result.HasError = true;
                    result.Error = ex.Message;
                    result.DetailedError = JsonConvert.SerializeObject(ex);
                }

                results.Add(result);
            }

            return Result.Combine(results);
        }

        public async Task<Result<User>> GetUserAsync(string memberEmail)
        {
            var result = new Result<User>();
            var uri = $"{ConstantsExtension.GraphApiVersion}/users?$filter=mail eq '{memberEmail}'&$select=id";

            var response = await httpClient.GetContentAsync(uri);
            var responseString = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                result.Value = JsonConvert.DeserializeObject<User>(responseString);
                return result;
            }

            result.HasError = true;
            result.Error = response.ReasonPhrase;
            result.DetailedError = responseString;

            return result;
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

            public CreateChannelGraphObject(User user)
            {
                memberId = user.Value.Any() ? user.Value[0].Id : "0";
            }
        }
    }
}
