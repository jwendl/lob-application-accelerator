using LobAccelerator.Library.Factories;
using LobAccelerator.Library.Interfaces;
using LobAccelerator.Library.Managers;
using LobAccelerator.Library.Models;
using System;
using System.Threading.Tasks;

namespace LobAccelerator.Library.Managers
{
    public class WorkflowManager
        : IWorkflowManager
    {
        private readonly ITeamsManager teamsManager;

        public WorkflowManager(string accessToken)
        {
            var httpClient = GraphClientFactory.CreateHttpClient(accessToken);
            teamsManager = new TeamsManager(httpClient);
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
