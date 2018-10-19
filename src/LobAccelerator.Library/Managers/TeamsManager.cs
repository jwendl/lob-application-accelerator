using LobAccelerator.Library.Extensions;
using LobAccelerator.Library.Managers.Interfaces;
using LobAccelerator.Library.Models.Teams;
using LobAccelerator.Library.Models.Teams.Channels;
using LobAccelerator.Library.Models.Teams.Groups;
using LobAccelerator.Library.Models.Teams.Results;
using LobAccelerator.Library.Models.Teams.Teams;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static LobAccelerator.Library.Extensions.ConstantsExtension;

namespace LobAccelerator.Library.Managers
{
    public class TeamsManager
        : ITeamsManager
    {
        private readonly HttpClient httpClient;
        private readonly Uri baseUri;
        private readonly string apiVersion;
        private readonly ILogger logger;
        private readonly IOneDriveManager oneDriveManager;

        public TeamsManager(HttpClient httpClient, ILogger logger, IOneDriveManager oneDriveManager)
        {
            this.httpClient = httpClient;
            this.logger = logger;

            baseUri = new Uri("https://graph.microsoft.com/");
            apiVersion = TeamsApiVersion;

            this.oneDriveManager = oneDriveManager;
        }

        public async Task<TeamResourceResults> CreateResourceAsync(TeamResource resource)
        {
            logger.LogInformation($"Starting to create the group {resource.DisplayName}");
            var group = await CreateGroupAsync(resource);
            logger.LogInformation($"Finished creating the group {resource.DisplayName}");

            logger.LogInformation($"Starting to create the team {resource.DisplayName}");
            var team = await CreateTeamAsync(group.Id, resource);
            logger.LogInformation($"Finished creating the team {resource.DisplayName}");

            logger.LogInformation($"Starting to create {resource.Channels.Count()} channels");
            var channels = await CreateChannelsAsync(team.Id, resource.Channels);
            logger.LogInformation($"Finished creating {resource.Channels.Count()} channels");

            logger.LogInformation($"Starting to create {resource.Members.Count()} members");
            await AddPeopleToChannelAsync(resource.Members, team.Id);
            logger.LogInformation($"Finished creating {resource.Members.Count()} members");

            logger.LogInformation($"Starting to copy files");
            var files = await CopyFilesToChannels(resource.Channels, team.Id);
            logger.LogInformation($"Finished copying files");

            return new TeamResourceResults()
            {
                Group = group,
                Team = team,
                Channels = channels,
                Files = files,
            };
        }

        private async Task<IEnumerable<FileResult>> CopyFilesToChannels(IEnumerable<ChannelResource> channels, string teamId)
        {
            await Task.Delay(16000);
            // TODO: Remove this call.
            // BUG: Creating Teams through Graph is taking too long to propagate the files directory properties.

            var results = new List<FileResult>();
            foreach (var channel in channels)
            {
                //TODO: Remove the following call when the bug of not creating a folder for a channel is fixed.
                await CreateChannelFolderOnGroupDocumentLibrary(teamId, channel.DisplayName);

                foreach (var resource in channel.Files)
                {
                    if (IsFile(resource))
                    {
                        await oneDriveManager.CopyFileFromOneDriveToTeams(teamId, channel.DisplayName, resource);
                    }
                    else
                    {
                        await oneDriveManager.CopyFolderFromOneDriveToTeams(teamId, channel.DisplayName, resource);
                    }

                    results.Add(new FileResult() { FilePath = resource, Message = $"File {resource} added..." });
                }
            }

            return results;
        }

        private async Task CreateChannelFolderOnGroupDocumentLibrary(string teamId, string channelName)
        {
            var url = $"https://graph.microsoft.com/beta/groups/{teamId}/drive/root/children/";

            var requestBody = new
            {
                name = channelName,
                folder = new { }
            };

            var requestBodyStr = JsonConvert.SerializeObject(requestBody);
            var body = new StringContent(requestBodyStr, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(url, body);
            response.EnsureSuccessStatusCode();
        }

        private bool IsFile(string input)
            => new System.Text.RegularExpressions.Regex(@"\.[a-zA-Z0-9]*$").IsMatch(input);

        /// <summary>
        /// Creates a new group where teams will be assigned to.
        /// </summary>
        /// <returns>Group ID</returns>
        public async Task<Group> CreateGroupAsync(TeamResource resource)
        {
            var groupUri = new Uri(baseUri, $"{apiVersion}/groups");

            var requestContent = new GroupBody
            {
                Description = resource.Description,
                DisplayName = resource.DisplayName,
                GroupTypes = new List<string> { GroupTypes.Unified.ToString() },
                MailEnabled = true,
                MailNickname = resource.MailNickname ?? $"{resource.DisplayName.ToLowerInvariant().Replace(' ', '-')}-{Guid.NewGuid()}",
                SecurityEnabled = false
            };

            logger.LogInformation($"Creating group using {groupUri} and {JsonConvert.SerializeObject(requestContent)}");
            var response = await httpClient.PostContentAsync(groupUri.AbsoluteUri, requestContent);
            var responseString = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<Group>(responseString);
        }

        /// <summary>
        /// Creates a new Team to an existing group.
        /// </summary>
        /// <returns></returns>
        public async Task<Team> CreateTeamAsync(string groupId, TeamResource resource)
        {
            var uri = new Uri(baseUri, $"{apiVersion}/groups/{groupId}/team");

            var requestContent = new TeamBody
            {
                MemberSettings = resource.MemberSettings,
                MessagingSettings = resource.MessagingSettings,
                FunSettings = resource.FunSettings
            };

            var response = await httpClient.PutContentAsync(uri.AbsoluteUri, requestContent);
            var responseString = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<Team>(responseString);
        }

        /// <summary>
        /// Create channels under a team.
        /// </summary>
        /// <param name="teamsId">Team ID</param>
        /// <param name="channels">List of channels to be added</param>
        /// <returns></returns>
        public async Task<IEnumerable<Channel>> CreateChannelsAsync(string teamId, IEnumerable<ChannelResource> channels)
        {
            var uri = new Uri(baseUri, $"{apiVersion}/teams/{teamId}/channels");

            var results = new List<Channel>();
            foreach (var channel in channels)
            {
                var response = await httpClient.PostContentAsync(uri.AbsoluteUri, channel);
                var responseString = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<Channel>(responseString);
                results.Add(result);
            }

            return results;
        }

        public async Task AddTabToChannelBasedOnUrlAsync(string tabName, string serviceUrl, string teamId, string channelId)
        {
            var addTabUrl = $"{GraphAlphaApiVersion}/teams/{teamId}/channels/{channelId}/tabs";
            var quickObject = new
            {
                name = tabName,
                teamsAppId = "com.microsoft.teamspace.tab.web",
                configuration = new
                {
                    entityId = string.Empty,
                    contentUrl = serviceUrl,
                    removeUrl = string.Empty,
                    websiteUrl = serviceUrl
                }
            };

            var response = await httpClient.PostContentAsync(addTabUrl, quickObject);
            await response.Content.ReadAsStringAsync();
        }


        public async Task AddPeopleToChannelAsync(IEnumerable<string> members, string teamId)
        {
            var addMemberUrl = $"{apiVersion}/groups/{teamId}/members/$ref";

            foreach (var member in members)
            {
                var addMemberBody = new AddGroupMemberBody(member);
                var response = await httpClient.PostContentAsync(addMemberUrl, addMemberBody);
                await response.Content.ReadAsStringAsync();
            }
        }

        public async Task<string> SearchTeamAsync(string displayName)
        {
            var listTeams = new Uri(baseUri, $"beta/groups?$filter=displayName eq '{displayName}'&$select=id");

            var response = await httpClient.GetAsync(listTeams);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var contentObj = JObject.Parse(content);

            return contentObj["value"][0]["id"].Value<string>();
        }

        public async Task DeleteChannelAsync(string groupId)
        {
            var deleteUri = new Uri(baseUri, $"{apiVersion}/groups/{groupId}");
            var deletePermanentUri = new Uri(baseUri, $"{apiVersion}/directory/deleteditems/microsoft.graph.group/{groupId}");

            var responseDelete = await httpClient.DeleteAsync(deleteUri);
            responseDelete.EnsureSuccessStatusCode();

            var responseDeletePerm = await httpClient.DeleteAsync(deletePermanentUri);
            responseDeletePerm.EnsureSuccessStatusCode();
        }
    }
}
