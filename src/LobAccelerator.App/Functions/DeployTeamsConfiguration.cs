using LobAccelerator.App.Model;
using LobAccelerator.App.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Linq;
using static LobAccelerator.App.Util.GlobalSettings;

namespace LobAccelerator.App.Functions
{
    public static class DeployTeamsConfiguration
    {
        //[Disable]
        [FunctionName("DeployTeamsConfiguration" )]
        public static void Run(
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
