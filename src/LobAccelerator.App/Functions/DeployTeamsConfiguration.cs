using LobAccelerator.App.Locators;
using LobAccelerator.App.Model;
using LobAccelerator.App.Models;
using LobAccelerator.Library.Interfaces;
using LobAccelerator.Library.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using static LobAccelerator.App.Util.GlobalSettings;

namespace LobAccelerator.App.Functions
{
    public static class DeployTeamsConfiguration
    {
        [FunctionName("DeployTeamsConfiguration")]
        public static async Task Run(
            [QueueTrigger(TEAMS_TASK_QUEUE)]
            string teamTask,
            [Table(PARAM_TABLE, PARAM_PARTITION_KEY, PARAM_TOKEN_ROW)]
            Parameter refreshToken,
            [Table(TEAM_TASK_TABLE, "TeamsTask", "{queueTrigger}")]
            TeamsConfiguration teamsConfig,
            //[Table(MEMBER_TASK_TABLE, "{queueTrigger}")]
            //IQueryable<MemeberConfiguration> teamsMembers,
            //[Table(CHANNEL_TASK_TABLE, "{queueTrigger}")]
            //IQueryable<ChannelConfiguration> teamsChannels,
            ILogger log)
        {
            log.LogInformation($"C# Queue trigger DeployTeamsConfiguration processed: {teamTask}");

            Workflow workflow = null; // TODO: Deserialize from Queue
            IWorkflowManager workflowManager = ServiceLocator.GetRequiredService<IWorkflowManager>();

            await workflowManager.CreateResourceAsync(workflow);
        }
    }
}
