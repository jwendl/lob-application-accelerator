using LobAccelerator.Library.Interfaces;
using LobAccelerator.Library.Models;
using System.Threading.Tasks;

namespace LobAccelerator.Library.Managers
{
    public class WorkflowManager
        : IWorkflowManager
    {
        private readonly ITeamsManager teamsManager;
        private readonly IOneDriveManager oneDriveManager;

        public WorkflowManager(ITeamsManager teamsManager, IOneDriveManager oneDriveManager)
        {
            this.teamsManager = teamsManager;
            this.oneDriveManager = oneDriveManager;
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
