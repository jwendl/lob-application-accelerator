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

        public WorkflowManager(string accessToken)
        {
            var httpClient = GraphClientFactory.CreateHttpClient(accessToken);

            teamsManager = new TeamsManager(httpClient);
        }

        public async Task CreateResourceAsync(Workflow resource)
        {
            foreach (var team in resource.Teams)
            {
                await teamsManager.CreateResourceAsync(team);
            }
        }

        Task<IResult> IResourceManager<Workflow>.CreateResourceAsync(Workflow resource)
        {
            throw new NotImplementedException();
        }
    }
}
