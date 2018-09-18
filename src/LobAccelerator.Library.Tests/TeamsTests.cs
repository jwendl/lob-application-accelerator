using LobAccelerator.Library.Factories;
using LobAccelerator.Library.Managers;
using LobAccelerator.Library.Models;
using LobAccelerator.Library.Models.Teams;
using LobAccelerator.Library.Tests.Utils.Auth;
using LobAccelerator.Library.Tests.Utils.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace LobAccelerator.Library.Tests
{
    public class TeamsTests
    {
        private readonly ConfigurationManager configuration;
        private readonly TokenRetriever tokenRetriever;

        public TeamsTests()
        {
            configuration = new ConfigurationManager();
            tokenRetriever = new TokenRetriever(configuration);
        }

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
                                DisplayName = "New Teams Channel",
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
            var team = CreateWorkflow(0).Teams.First();
            HttpClient httpClient = await GetHttpClient();
            var teamsManager = new TeamsManager(httpClient);

            //Act
            var result = await teamsManager.CreateGroupAsync(team);

            //Assert
            Assert.False(result.HasError);
        }

        [Fact]
        public async Task AddNewTeam()
        {
            //Arrange
            var team = CreateWorkflow(1).Teams.First();
            HttpClient httpClient = await GetHttpClient();
            var teamsManager = new TeamsManager(httpClient);

            //Act
            var groupResult = await teamsManager.CreateGroupAsync(team);
            var teamResult = await teamsManager.CreateTeamAsync(groupResult.Value.Id, team);

            //Assert
            Assert.False(teamResult.HasError);
        }

        [Fact]
        public async Task AddNewChannels()
        {
            //Arrange
            var team = CreateWorkflow(2).Teams.First();
            HttpClient httpClient = await GetHttpClient();
            var teamsManager = new TeamsManager(httpClient);

            //Act
            var groupResult = await teamsManager.CreateGroupAsync(team);
            var teamResult = await teamsManager.CreateTeamAsync(groupResult.Value.Id, team);
            var channelsResult = await teamsManager.CreateChannelsAsync(teamResult.Value.Id, team.Channels);

            //Assert
            Assert.False(channelsResult.HasError());
        }

        [Fact]
        public async Task AddPeopleToChannel()
        {
            //Arrange
            var workflow = CreateWorkflow(3);
            var team = CreateWorkflow(3).Teams.First();
            HttpClient httpClient = await GetHttpClient();
            var teamsManager = new TeamsManager(httpClient);

            //Act
            var groupResult = await teamsManager.CreateGroupAsync(team);
            var teamResult = await teamsManager.CreateTeamAsync(groupResult.Value.Id, team);
            var membersResult = await teamsManager.AddPeopleToChannelAsync(team.Members, teamResult.Value.Id);

            //Assert
            Assert.False(membersResult.HasError());
        }

        [Fact]
        public async Task AddAllResources()
        {
            //Arrange
            var team = CreateWorkflow(4).Teams.First();
            HttpClient httpClient = await GetHttpClient();
            var teamsManager = new TeamsManager(httpClient);

            //Act
            var result = await teamsManager.CreateResourceAsync(team);

            //Assert
            Assert.False(result.HasError());
        }

        private async Task<HttpClient> GetHttpClient()
        {
            var token = await tokenRetriever.GetTokenByAuthorizationCodeFlowAsync(
                "Group.ReadWrite.All", 
                "User.ReadBasic.All");
            var httpClient = GraphClientFactory.CreateHttpClient(token.access_token);
            return httpClient;
        }
    }
}
