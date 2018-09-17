using LobAccelerator.Library.Interfaces;
using LobAccelerator.Library.Managers;
using LobAccelerator.Library.Models;
using System;
using System.Threading.Tasks;

namespace LobAccelerator.Manager.Library
{
    public class WorkflowManager : IWorkflowManager
    {
        private readonly ITeamsManager teamsManager;

        public WorkflowManager()
        {
            teamsManager = new TeamsManager();
        }

        public async Task CreateResourceAsync(Workflow resource)
        {
            await teamsManager.CreateResourceAsync(resource.Teams);
        }
    }
}
