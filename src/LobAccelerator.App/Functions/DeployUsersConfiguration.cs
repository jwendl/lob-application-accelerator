using LobAccelerator.App.Locators;
using LobAccelerator.App.Models;
using LobAccelerator.Library.Managers.Interfaces;
using LobAccelerator.Library.Models.Users;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using static LobAccelerator.App.Extensions.ConstantsExtension;

namespace LobAccelerator.App.Functions
{
    public static class DeployUsersConfiguration
    {
        [FunctionName("DeployUsersConfiguration")]
        public static async Task Run([QueueTrigger(USERS_REQUEST_QUEUE)] UserResource userResource, [Table(PARAM_TABLE, PARAM_PARTITION_KEY, PARAM_TOKEN_ROW)] Parameter accessToken, ILogger log)
        {
            ServiceLocator.BuildServiceProvider(log, accessToken.Value);

            var usersManager = ServiceLocator.GetRequiredService<IUserManager>();
            var results = await usersManager.CreateResourceAsync(userResource);

            log.LogInformation($"C# Queue trigger DeployUsersConfiguration processed: {userResource}");
            log.LogInformation($"{userResource.DisplayName} user created!");
        }
    }
}
