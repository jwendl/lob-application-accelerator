using LobAccelerator.Library.Handlers;
using LobAccelerator.Library.Managers;
using LobAccelerator.Library.Models;
using LobAccelerator.Library.Models.Teams;
using LobAccelerator.Library.Services;
using LobAccelerator.Library.Services.Interfaces;
using LobAccelerator.Library.Tests.Utils.Auth;
using LobAccelerator.Library.Tests.Utils.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using NSubstitute;
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
        private readonly IServiceProvider serviceProvider;

        public TeamsTests()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddScoped<ILogger, ConsoleLogger>();
            serviceCollection.AddScoped<IConfiguration, ConfigurationManager>();
            serviceCollection.AddScoped<ITokenCacheService, TokenCacheService>();
            serviceCollection.AddScoped<ITokenRetriever, TokenRetriever>();
            serviceProvider = serviceCollection.BuildServiceProvider();
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
            var teamsManager = await CreateTeamsManagerAsync();

            //Act
            await teamsManager.CreateGroupAsync(team);

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
            var teamsManager = await CreateTeamsManagerAsync();

            //Act
            var groupResult = await teamsManager.CreateGroupAsync(team);
            var teamResult = await teamsManager.CreateTeamAsync(groupResult.Id, team);

            //Assert

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
            var teamsManager = await CreateTeamsManagerAsync();

            //Act
            var groupResult = await teamsManager.CreateGroupAsync(team);
            var teamResult = await teamsManager.CreateTeamAsync(groupResult.Id, team);
            var channelsResult = await teamsManager.CreateChannelsAsync(teamResult.Id, team.Channels);

            //Assert
            Assert.True(channelsResult.Any());

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
            var teamsManager = await CreateTeamsManagerAsync();

            //Act
            var groupResult = await teamsManager.CreateGroupAsync(team);
            var teamResult = await teamsManager.CreateTeamAsync(groupResult.Id, team);
            await teamsManager.AddPeopleToChannelAsync(team.Members, teamResult.Id);

            //Assert

            //Teardown
            var groupId = await teamsManager.SearchTeamAsync(team.DisplayName);
            await teamsManager.DeleteChannelAsync(groupId);
        }

        [Fact]
        public async Task AddInvalidPeopleToChannel()
        {
            //Arrange
            var teamNumber = new Random().Next();
            var team = CreateWorkflow(teamNumber).Teams.First();
            team.Members = new List<string>()
            {
                "jwendl@jwazuread.onmicrosoft.com",
                "testuser001@jwazuread.onmicrosoft.com",
                "user@othertenat.onmicrosoft.com"
            };

            var teamsManager = await CreateTeamsManagerAsync();

            //Act
            var groupResult = await teamsManager.CreateGroupAsync(team);
            var teamResult = await teamsManager.CreateTeamAsync(groupResult.Id, team);
            await teamsManager.AddPeopleToChannelAsync(team.Members, teamResult.Id);

            //Assert

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
            var teamsManager = await CreateTeamsManagerAsync();

            //Act
            var result = await teamsManager.CreateResourceAsync(team);

            //Assert

            //Teardown
            var groupId = await teamsManager.SearchTeamAsync(team.DisplayName);
            await teamsManager.DeleteChannelAsync(groupId);
        }

        [Fact]
        public async Task AddFilesToChannel()
        {
            //Arrange
            var teamNumber = new Random().Next();
            var team = CreateWorkflow(teamNumber).Teams.First();
            team.Channels.First().Files = new List<string>()
            {
                "TransferFiles/Welcome/Introduction/WelcomeCSE.pptx",
                "TransferFiles/Welcome/Docs",
                "Hotel.xlsx"
            };
            var teamsManager = await CreateTeamsManagerAsync();

            //Act
            var result = await teamsManager.CreateResourceAsync(team);

            //Assert

            //Teardown
            var groupId = await teamsManager.SearchTeamAsync(team.DisplayName);
            await teamsManager.DeleteChannelAsync(groupId);
        }

        private async Task<TeamsManager> CreateTeamsManagerAsync()
        {
            var httpClient = await GetHttpClientAsync();
            var logger = Substitute.For<ILogger>();
            var oneDriveManager = new OneDriveManager(httpClient);
            var teamsManager = new TeamsManager(httpClient, logger, oneDriveManager);
            return teamsManager;
        }

        private async Task<HttpClient> GetHttpClientAsync()
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var tokenRetriever = serviceProvider.GetRequiredService<ITokenRetriever>();
            var tokenCacheService = serviceProvider.GetRequiredService<ITokenCacheService>();

            var scopes = new string[] {
                $"api://{configuration["ClientId"]}/access_as_user"
            };
            var log = new ConsoleLogger("Default", null, true);
            var tokenManager = new TokenManager(configuration, tokenCacheService, log);
            var token = await tokenRetriever.GetTokenByAuthorizationCodeFlowAsync(scopes);
            var uri = await tokenManager.GetAuthUriAsync(scopes);
            var authCode = await tokenRetriever.GetAuthCodeByMsalUriAsync(uri);
            var authResult = await tokenManager.GetAccessTokenFromCodeAsync(authCode, scopes);
            var tokenManagerHttpMessageHandler = new TokenManagerHttpMessageHandler(log, tokenManager, authResult.AccessToken);
            var httpClient = new HttpClient(tokenManagerHttpMessageHandler);
            httpClient.BaseAddress = new Uri(configuration["GraphBaseUri"]);
            var desiredScopes = new string[]
            {
                "Group.ReadWrite.All",
                "User.ReadBasic.All"
            };
            httpClient.DefaultRequestHeaders.Add("X-LOBScopes", desiredScopes);
            return httpClient;
        }
    }
}
