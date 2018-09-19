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
        //[Disable]
        [FunctionName("DeployTeamsConfiguration" )]
        public static void Run(
            [QueueTrigger(REQUEST_QUEUE)]
            TeamsJsonConfiguration teamTask,
            [Table(PARAM_TABLE, PARAM_PARTITION_KEY, PARAM_TOKEN_ROW)]
            Parameter refreshToken,
            ILogger log)
        {
            log.LogInformation($"C# Queue trigger DeployTeamsConfiguration processed: {teamTask}");

            var teamsManager = ServiceLocator.GetRequiredService<ITeamsManager>();

            //TODO: Call refresh Token Functionality
            //Hey use me :: refreshToken

            //TODO: Create teams group
            //Hey use me :: teamsConfig


            //foreach (var member in teamsMembers)
            //{
            //    //TODO: Create members for team
            //    //Hey use me :: teamsMembers
            //}

            //foreach (var channel in teamsChannels)
            //{
            //    //TODO: Create channels for team
            //    var usersForTheChannel = channel.GetMembersList();
            //    //TODO: Add the suers to the channel

            //}

        }
    }
}
