using LobAccelerator.App.Locators;
using LobAccelerator.App.Models;
using LobAccelerator.Library.Managers.Interfaces;
using LobAccelerator.Library.Models.Azure;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using static LobAccelerator.App.Extensions.ConstantsExtension;

namespace LobAccelerator.App.Functions
{
    public static class DeployARMConfiguration
    {
        [FunctionName("DeployARMConfiguration")]
        public static async Task Run([QueueTrigger(ARM_REQUEST_QUEUE)] ARMDeployment armDeployment, [Table(PARAM_TABLE, PARAM_PARTITION_KEY, PARAM_TOKEN_ROW)] Parameter accessToken, ILogger log)
        {
            ServiceLocator.BuildServiceProvider(log, accessToken.Value);

            var azureManager = ServiceLocator.GetRequiredService<IAzureManager>();
            var results = await azureManager.DeployARMTemplateAsync(armDeployment);

            log.LogInformation($"C# Queue trigger DeployARMConfiguration processed: {armDeployment}");
            log.LogInformation($"{armDeployment.Name} deployed!");
        }
    }
}
