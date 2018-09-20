using LobAccelerator.Library.Interfaces;
using LobAccelerator.Library.Models;
using System.Threading.Tasks;

namespace LobAccelerator.Library.Managers
{
    public class WorkflowManager
        : IWorkflowManager
    {
        private readonly ITeamsManager teamsManager;

        public WorkflowManager(ITeamsManager teamsManager)
        {
            this.teamsManager = teamsManager;
        }

        public async Task CreateAllResourceAsync(Workflow resource)
        {
            foreach (var team in resource.Teams)
            {
                await teamsManager.CreateResourceAsync(team);
            }
        }

        public async Task<IResult> CreateResourceAsync(Workflow resource)
        {
            await CreateAllResourceAsync(resource);
            return null;
        }
    }
}
