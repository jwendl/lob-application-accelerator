using LobAccelerator.App.Model;
using LobAccelerator.Library.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LobAccelerator.App
{
    public static class CreateTasksTree
    {
        const string QUEUE_NAME = "teams-requested-tasks";

        [FunctionName("CreateTasksTree")]
        public static void Run(
            [QueueTrigger(QUEUE_NAME)]
            string queueMsg,
            [Table("teamtask")]out TeamsConfiguration teamEntry,
            [Table("membertask")] ICollector<MemeberConfiguration> memberEntries,
            [Table("channeltask")] ICollector<ChannelConfiguration> channelEntries,
            [Queue("pending-team-tasks")] out CloudQueueMessage newtask,
            ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {queueMsg}");

            var teamConfig = JsonConvert.DeserializeObject<TeamsJsonConfiguration>(queueMsg);

            //create an entry in table for teamconfiguration
            teamEntry = CreateTeamEntry(teamConfig);
            var memberlist= CreateMemberEntries(teamConfig, teamEntry);
            foreach (var m in memberlist)
            {
                memberEntries.Add(m);
            }

            var channellist = CreateChannelEntries(teamConfig, teamEntry);
            foreach (var c in channellist)
            {
                channelEntries.Add(c);
            }
            newtask = new CloudQueueMessage(teamEntry.RowKey);
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
