using LobAccelerator.App.Locators;
using LobAccelerator.App.Models;
using LobAccelerator.Library.Interfaces;
using LobAccelerator.Library.Models;
using Microsoft.Azure.WebJobs;
using System.Linq;
using Microsoft.Extensions.Logging;
using static LobAccelerator.App.Extensions.ConstantsExtension;

namespace LobAccelerator.App.Functions
{
    public static class DeployTeamsConfiguration
    {
        [FunctionName("DeployTeamsConfiguration")]
        public static void Run(
            [QueueTrigger(REQUEST_QUEUE)]
            Workflow workflow,
            [Table(PARAM_TABLE, PARAM_PARTITION_KEY, PARAM_TOKEN_ROW)]
            Parameter accessToken,
            ILogger log)
        {
            ServiceLocator.BuildServiceProvider(accessToken.Value);
            var workflowManager = ServiceLocator.GetRequiredService<IWorkflowManager>();
           
            workflowManager.CreateResourceAsync(workflow);
            var tmpTeam = workflow.Teams.FirstOrDefault();
            log.LogInformation($"C# Queue trigger DeployTeamsConfiguration processed: {workflow}");
            log.LogInformation($"{tmpTeam.DisplayName} team created, {tmpTeam.Channels.Count()} channels, {tmpTeam.Members.Count()} members and configurations Done!");
        }
    }
}
