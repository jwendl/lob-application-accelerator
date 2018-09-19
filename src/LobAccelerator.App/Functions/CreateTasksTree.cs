using LobAccelerator.App.Model;
using LobAccelerator.Library.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using LobAccelerator.App;
using static LobAccelerator.App.Util.GlobalSettings;


namespace LobAccelerator.App.Functions
{
    public static class CreateTasksTree
    {
        [FunctionName("CreateTasksTree")]
        public static void Run(
            [QueueTrigger(REQUEST_QUEUE)]
            string queueMsg,
            [Table(TEAM_TASK_TABLE)]
            out TeamsConfiguration teamEntry,
            [Table(MEMBER_TASK_TABLE)]
            ICollector<MemeberConfiguration> memberEntries,
            [Table(CHANNEL_TASK_TABLE)]
            ICollector<ChannelConfiguration> channelEntries,
            [Queue(TEAMS_TASK_QUEUE)]
            out CloudQueueMessage newTeamsTask,
            //[Queue(TEAMS_TASK_QUEUE)]
            //CloudQueue  taskQueue,
            ILogger log)
        {
            log.LogInformation($"CreateTasksTree trigger function processed for requested Id: {queueMsg}");

            var teamConfig = JsonConvert.DeserializeObject<TeamsJsonConfiguration>(queueMsg);

            //create an entry in table for teamconfiguration
            teamEntry = CreateTeamEntry(teamConfig);
            
            var memberlist = CreateMemberEntries(teamConfig, teamEntry);
            foreach (var m in memberlist)
            {
                memberEntries.Add(m);
            }

            var channellist = CreateChannelEntries(teamConfig, teamEntry);
            foreach (var c in channellist)
            {
                channelEntries.Add(c);
            }

            //taskQueue.AddMessageAsync(new CloudQueueMessage(teamEntry.RowKey)).Wait();

            newTeamsTask = new CloudQueueMessage(teamEntry.RowKey);
        }

        private static TeamsConfiguration CreateTeamEntry(TeamsJsonConfiguration teamConfig)
        {
            return new TeamsConfiguration()
            {
                PartitionKey = "TeamsTask",
                RowKey = Guid.NewGuid().ToString(),
                Name = teamConfig.Name,
                Description = teamConfig.Description
            };
        }

        private static List<MemeberConfiguration> CreateMemberEntries(TeamsJsonConfiguration teamConfig, TeamsConfiguration teamEntry)
        {
            var lista = new List<MemeberConfiguration>();
            foreach (var metaMember in teamConfig.Members)
            {
                var member = new MemeberConfiguration()
                {
                    PartitionKey = teamEntry.RowKey,
                    RowKey = Guid.NewGuid().ToString(),
                    Name = metaMember
                };

                lista.Add(member);
            }
            return lista;
        }

        private static List<ChannelConfiguration> CreateChannelEntries(
            TeamsJsonConfiguration teamConfig,
            TeamsConfiguration teamEntry)
        {
            var lista = new List<ChannelConfiguration>();
            foreach (var metaChannel in teamConfig.Channels)
            {
                var channel = new ChannelConfiguration()
                {
                    PartitionKey = teamEntry.RowKey,
                    RowKey = Guid.NewGuid().ToString(),
                    Name = metaChannel.Name,
                    Description = metaChannel.Description,
                };
                channel.MemberList = CreateMemberStringFromConfig(metaChannel.Members);
                lista.Add(channel);
            }
            return lista;
        }

        private static string CreateMemberStringFromConfig(List<string> members)
        {
            var sb = new StringBuilder();
            foreach (var m in members)
            {
                sb.Append($"{m};");
            }

            return sb.ToString();
        }
    }
}
