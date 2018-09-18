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

        public static Workflow Workflow => new Workflow()
        {
            Teams = new List<TeamResource>()
                {
                    new TeamResource()
                    {
                        DisplayName = "New Teams Team",
                        Description = "This is a team for teams.",
                        MailNickname = "group",
                        Members = new List<string>()
                        {
                            "juswen@microsoft.com",
                            "ridize@microsoft.com",
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


        [Fact]
        public async Task AddNewGroup()
        {
            //Arrange
            var team = Workflow.Teams.First();
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
            var team = Workflow.Teams.First();
            HttpClient httpClient = await GetHttpClient();
            var teamsManager = new TeamsManager(httpClient);

            //Act
            var groupResult = await teamsManager.CreateGroupAsync(team);
            var teamResult = await teamsManager.CreateTeamAsync(groupResult.Value.Id, team);

            //Assert
            Assert.False(teamResult.HasError);
        }

        [Fact]
        public async Task AddPeopleToChannel()
        {
            //Arrange
            var teamId = Workflow.Teams.First().DisplayName;
            var members = Workflow.Teams.First().Members;
            HttpClient httpClient = await GetHttpClient();
            var teamsManager = new TeamsManager(httpClient);

            //Act
            await teamsManager.AddPeopleToChannelAsync(members, teamId);

            //Assert
        }

        private async Task<HttpClient> GetHttpClient()
        {
            var token = await tokenRetriever.GetTokenByAuthorizationCodeFlowAsync("Group.ReadWrite.All");
            var httpClient = GraphClientFactory.CreateHttpClient(token.access_token);
            return httpClient;
        }
    }
}
