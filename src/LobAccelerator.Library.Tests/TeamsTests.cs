using LobAccelerator.Library.Factories;
using LobAccelerator.Library.Managers;
using LobAccelerator.Library.Models;
using LobAccelerator.Library.Models.Teams;
using LobAccelerator.Library.Tests.Utils.Auth;
using LobAccelerator.Library.Tests.Utils.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace LobAccelerator.Library.Tests
{
    public class TeamsTests
    {
        private static readonly ConfigurationManager configurationManager =
            new ConfigurationManager();
        private static readonly TokenRetriever tokenRetriever
            = new TokenRetriever(configurationManager);

        public Workflow CreateWorkflow(int teamNumber)
        {
            return new Workflow
            {
                Teams = new List<TeamResource>()
                {
                    new TeamResource()
                    {
                        DisplayName = $"Team {teamNumber}",
                        Description = $"This is a testing team {teamNumber}.",
                        MailNickname = $"group{teamNumber}",
                        Members = new List<string>()
                        {
                            "jwendl@jwazuread.onmicrosoft.com",
                            "testuser001@jwazuread.onmicrosoft.com",
                        },
                        MemberSettings = new MemberSettings()
                        {
                            AllowCreateUpdateChannels = true,
                        },
                        MessagingSettings = new MessagingSettings()
                        {
                            AllowUserEditMessages = true,
                            AllowUserDeleteMessages = false,
                        },
                        FunSettings = new FunSettings()
                        {
                            AllowGiphy = true,
                            GiphyContentRating = "strict",
                        },
                        Channels = new List<ChannelResource>()
                        {
                            new ChannelResource
                            {
                                DisplayName = "New Teams Channel 2",
                                Description = "A new channel for the teams team."
                            }
                        }
                    }
                }
            };
        }
        
        [Fact]
        public async Task AddNewGroup()
        {
            //Arrange
            var teamNumber = new Random().Next();
            var team = CreateWorkflow(teamNumber).Teams.First();
            HttpClient httpClient = await GetHttpClient();
            var teamsManager = new TeamsManager(httpClient);

            //Act
            var result = await teamsManager.CreateGroupAsync(team);

            //Assert
            Assert.False(result.HasError);

            //Teardown
            var groupId = await teamsManager.SearchTeamAsync(team.DisplayName);
            await teamsManager.DeleteChannelAsync(groupId);
        }

        [Fact]
        public async Task AddNewTeam()
        {
            //Arrange
            var teamNumber = new Random().Next();
            var team = CreateWorkflow(teamNumber).Teams.First();
            HttpClient httpClient = await GetHttpClient();
            var teamsManager = new TeamsManager(httpClient);

            //Act
            var groupResult = await teamsManager.CreateGroupAsync(team);
            var teamResult = await teamsManager.CreateTeamAsync(groupResult.Value.Id, team);

            //Assert
            Assert.False(teamResult.HasError);

            //Teardown
            var groupId = await teamsManager.SearchTeamAsync(team.DisplayName);
            await teamsManager.DeleteChannelAsync(groupId);
        }

        [Fact]
        public async Task AddNewChannels()
        {
            //Arrange
            var teamNumber = new Random().Next();
            var team = CreateWorkflow(teamNumber).Teams.First();
            HttpClient httpClient = await GetHttpClient();
            var teamsManager = new TeamsManager(httpClient);

            //Act
            var groupResult = await teamsManager.CreateGroupAsync(team);
            var teamResult = await teamsManager.CreateTeamAsync(groupResult.Value.Id, team);
            var channelsResult = await teamsManager.CreateChannelsAsync(teamResult.Value.Id, team.Channels);

            //Assert
            Assert.False(channelsResult.HasError());

            //Teardown
            var groupId = await teamsManager.SearchTeamAsync(team.DisplayName);
            await teamsManager.DeleteChannelAsync(groupId);
        }

        [Fact]
        public async Task AddPeopleToChannel()
        {
            //Arrange
            var teamNumber = new Random().Next();
            var team = CreateWorkflow(teamNumber).Teams.First();
            
            HttpClient httpClient = await GetHttpClient();
            var teamsManager = new TeamsManager(httpClient);

            //Act
            var groupResult = await teamsManager.CreateGroupAsync(team);
            var teamResult = await teamsManager.CreateTeamAsync(groupResult.Value.Id, team);
            var membersResult = await teamsManager.AddPeopleToChannelAsync(team.Members, teamResult.Value.Id);

            //Assert
            Assert.False(membersResult.HasError());

            //Teardown
            var groupId = await teamsManager.SearchTeamAsync(team.DisplayName);
            await teamsManager.DeleteChannelAsync(groupId);
        }

        [Fact]
        public async Task AddAllResources()
        {
            //Arrange
            var teamNumber = new Random().Next();
            var team = CreateWorkflow(teamNumber).Teams.First();
            HttpClient httpClient = await GetHttpClient();
            var teamsManager = new TeamsManager(httpClient);

            //Act
            var result = await teamsManager.CreateResourceAsync(team);

            //Assert
            Assert.False(result.HasError());

            //Teardown
            var groupId = await teamsManager.SearchTeamAsync(team.DisplayName);
            await teamsManager.DeleteChannelAsync(groupId);
        }

        private async Task<HttpClient> GetHttpClient()
        {
            string[] scopes =
            {
                "Group.ReadWrite.All",
                "User.ReadBasic.All"
            };
            var token = await tokenRetriever.GetTokenByAuthorizationCodeFlowAsync(scopes);
            var tokenManager = new TokenManager(configurationManager);
            var httpClient = GraphClientFactory.CreateHttpClient(tokenManager, token.access_token);
            httpClient.DefaultRequestHeaders.Add("X-TMScopes", scopes);
            return httpClient;
        }
    }
}
