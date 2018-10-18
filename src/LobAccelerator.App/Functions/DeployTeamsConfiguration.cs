using LobAccelerator.App.Locators;
using LobAccelerator.App.Models;
using LobAccelerator.Library.Managers.Interfaces;
using LobAccelerator.Library.Models.Teams;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using static LobAccelerator.App.Extensions.ConstantsExtension;

namespace LobAccelerator.App.Functions
{
    public static class DeployTeamsConfiguration
    {
        [FunctionName("DeployTeamsConfiguration")]
        public static async Task Run([QueueTrigger(TEAMS_REQUEST_QUEUE)] TeamResource teamResource, [Table(PARAM_TABLE, PARAM_PARTITION_KEY, PARAM_TOKEN_ROW)] Parameter accessToken, ILogger log)
        {
            ServiceLocator.BuildServiceProvider(log, accessToken.Value);

            var teamsManager = ServiceLocator.GetRequiredService<ITeamsManager>();
            var results = await teamsManager.CreateResourceAsync(teamResource);

            log.LogInformation($"C# Queue trigger DeployTeamsConfiguration processed: {teamResource}");
            log.LogInformation($"{teamResource.DisplayName} team created, {teamResource.Channels.Count()} channels, {teamResource.Members.Count()} members and configurations Done!");
        }
    }
}
