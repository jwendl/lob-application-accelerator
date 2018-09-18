using LobAccelerator.Library.Factories;
using LobAccelerator.Library.Managers;
using LobAccelerator.Library.Models;
using LobAccelerator.Library.Models.Teams;
using LobAccelerator.Utils.Auth;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace LobAccelerator.Library.Tests
{
    public class TeamsTests
    {
        public static Workflow Workflow => new Workflow()
        {
            Teams = new List<Team>()
                {
                    new Team()
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
                        Channels = new List<Channel>()
                        {
                            new Channel()
                            {
                                DisplayName = "New Teams Channel",
                                Description = "A new channel for the teams team."
                            }
                        }
                    }
                }
        };

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
            var accessToken = await TokenRetriever.GetAccessTokenByAuthorizationCode();
            var httpClient = GraphClientFactory.CreateHttpClient(accessToken);
            return httpClient;
        }
    }
}
