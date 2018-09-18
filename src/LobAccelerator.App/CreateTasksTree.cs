using System;
using LobAccelerator.Library.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LobAccelerator.App
{
    public static class CreateTasksTree
    {
        const string QUEUE_NAME = "teams-requested-tasks";

        [FunctionName("CreateTasksTree")]
        public static void Run(
            [QueueTrigger(QUEUE_NAME)]
            string queueMsg, 
            ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {queueMsg}");

            var teamConfig = JsonConvert.DeserializeObject<TeamsJsonConfiguration>(queueMsg);

            //create an entry in table for teamconfiguration


        }
    }
}
