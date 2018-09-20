using LobAccelerator.Library.Configuration;
using LobAccelerator.Library.Factories;
using LobAccelerator.Library.Interfaces;
using LobAccelerator.Library.Models;
using System;
using System.Threading.Tasks;

namespace LobAccelerator.Library.Managers
{
    public class WorkflowManager
        : IWorkflowManager
    {
        private readonly ITeamsManager teamsManager;
        private readonly IOneDriveManager oneDriveManager;
        private readonly ITokenManager tokenManager;
        private readonly ConfigurationManager configurationManager;

        public WorkflowManager(string accessToken)
        {
            configurationManager = new ConfigurationManager();
            tokenManager = new TokenManager(configurationManager);
            var httpClient = GraphClientFactory.CreateHttpClient(tokenManager, accessToken);

            teamsManager = new TeamsManager(httpClient);
            oneDriveManager = new OneDriveManager(httpClient);
        }

        public async Task CreateAllResourceAsync(Workflow resource)
        {
            foreach (var team in resource.Teams)
                await teamsManager.CreateResourceAsync(team);
        }

        public async Task<IResult> CreateResourceAsync(Workflow resource)
        {
            await CreateAllResourceAsync(resource);
            return null;
        }
    }
}
