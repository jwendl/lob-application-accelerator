using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using System.Threading.Tasks;
using static LobAccelerator.App.Util.GlobalSettings;


namespace LobAccelerator.App.Functions
{
    public static class CleaningUp
    {
        /// <summary>
        /// Be Aare , this function requires admin token instead of function token
        /// </summary>
        /// <param name="req"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("CleaningUp")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Admin, "get", Route = null)]
            HttpRequest req, ILogger log,
            [Table(PARAM_TABLE)]
            CloudTable parameterTable,
            [Table(TEAM_TASK_TABLE)]
            CloudTable teamTaskTable,
            [Table(MEMBER_TASK_TABLE)]
            CloudTable memberTaskTable,
            [Table(CHANNEL_TASK_TABLE)]
            CloudTable channelTaskTable,
            [Queue(TEAMS_TASK_QUEUE)]
            CloudQueue teamsTaskQueue,
            [Queue(REQUEST_QUEUE)]
            CloudQueue  requestQueue)
        {

            log.LogInformation("C# HTTP trigger function processed a request.");

            await DeleteAllTables(parameterTable, teamTaskTable, memberTaskTable, channelTaskTable);

            await teamsTaskQueue.DeleteIfExistsAsync();
            await requestQueue.DeleteIfExistsAsync();

            return (ActionResult)new OkObjectResult($"Table and Queue Storage has been cleaned");
        }

        private static async Task DeleteAllTables(params CloudTable[] tables)
        {
            foreach (var table in tables)
            {
                await table.DeleteIfExistsAsync();
            }
        }
    }
}
