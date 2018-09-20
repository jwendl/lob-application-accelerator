using LobAccelerator.App.Locators;
using LobAccelerator.App.Models;
using LobAccelerator.Library.Interfaces;
using LobAccelerator.Library.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using static LobAccelerator.App.Util.GlobalSettings;

namespace LobAccelerator.App.Functions
{
    public static class DeployTeamsConfiguration
    {
        [FunctionName("DeployTeamsConfiguration")]
        public static void Run(
            [QueueTrigger(REQUEST_QUEUE)]
            Workflow workflow,
            [Table(PARAM_TABLE, PARAM_PARTITION_KEY, PARAM_TOKEN_ROW)]
            Parameter refreshToken,
            ILogger log)
        {
            log.LogInformation($"C# Queue trigger DeployTeamsConfiguration processed: {workflow}");

            IWorkflowManager workflowManager = ServiceLocator.GetRequiredService<IWorkflowManager>();

            workflowManager.CreateResourceAsync(workflow);
        }
    }
}
