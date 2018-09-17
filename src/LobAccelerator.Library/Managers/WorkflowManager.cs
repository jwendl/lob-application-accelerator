using LobAccelerator.Library.Interfaces;
using LobAccelerator.Library.Managers;
using LobAccelerator.Library.Models;
using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace LobAccelerator.Manager.Library
{
    public class WorkflowManager : IWorkflowManager
    {
        private readonly ITeamsManager teamsManager;

        public WorkflowManager() // AuthenticationHeaderValue authenticationHeaderValue)
        {
            teamsManager = new TeamsManager();
        }

        public async Task CreateResourceAsync(Workflow resource)
        {
            foreach (var team in resource.Teams)
                await teamsManager.CreateResourceAsync(team);
        }
    }
}
