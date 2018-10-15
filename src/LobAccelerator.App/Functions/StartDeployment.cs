using LobAccelerator.App.Locators;
using LobAccelerator.App.Models;
using LobAccelerator.Library.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using static LobAccelerator.App.Extensions.ConstantsExtension;

namespace LobAccelerator.App.Functions
{
    public static class StartDeployment
    {
        [FunctionName("StartDeployment")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestMessage req,
            [Table(PARAM_TABLE, PARAM_PARTITION_KEY, PARAM_TOKEN_ROW)] Parameter parameter,
            [Table(PARAM_TABLE, PARAM_PARTITION_KEY)] IAsyncCollector<Parameter> tokenParameters,
            [Queue(TEAMS_REQUEST_QUEUE)] CloudQueue teamsRequestQueue,
            [Queue(USERS_REQUEST_QUEUE)] CloudQueue usersRequestQueue,
            [Queue(ARM_REQUEST_QUEUE)] CloudQueue armRequestQueue,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function has completely processed a request.");

            var accessToken = req.Headers.Authorization?.Parameter;
            ServiceLocator.BuildServiceProvider(log, accessToken);

            var json = await req.Content.ReadAsStringAsync();
            var jObject = JObject.Parse(json);
            var fileText = File.ReadAllText(@"Schemas\workflow.schema.json");
            var jsonSchema = JSchema.Parse(fileText);
            var isValid = jObject.IsValid(jsonSchema, out IList<ValidationError> validationErrors);

            log.LogInformation($"C# HTTP trigger function processing a {(!isValid ? "in" : string.Empty)}valid request.");
            if (isValid)
            {
                var workflow = await req.Content.ReadAsAsync<Workflow>();
                parameter = await CreateOrUpdateTokenParameter(parameter, tokenParameters, accessToken);

                foreach (var team in workflow.Teams)
                {
                    var requestBody = JsonConvert.SerializeObject(team);
                    await usersRequestQueue.AddMessageAsync(new CloudQueueMessage(requestBody));
                }
                log.LogInformation($"{workflow.Users.Count()} Users have been scheduled for creation");

                // Populate arm template queue
                foreach (var armTemplate in workflow.ARMDeployments)
                {
                    var requestBody = JsonConvert.SerializeObject(armTemplate);
                    await armRequestQueue.AddMessageAsync(new CloudQueueMessage(requestBody));
                }
                log.LogInformation($"{workflow.ARMDeployments.Count()} ARM Deployments have been scheduled for creation");

                // Populate teams queue
                foreach (var team in workflow.Teams)
                {
                    var requestBody = JsonConvert.SerializeObject(team);
                    await teamsRequestQueue.AddMessageAsync(new CloudQueueMessage(requestBody));
                }
                log.LogInformation($"{workflow.Teams.Count()} Teams has been scheduled for creation");

                return new OkObjectResult($"{workflow.Teams.Count()} Teams are scheduled for creation, {workflow.Users.Count()} Users are scheduled for creation and {workflow.ARMDeployments.Count()} ARM Deployments are scheduled for creation.");
            }

            var errorJson = JsonConvert.SerializeObject(validationErrors);
            return new BadRequestObjectResult($"Invalid HttpRequest, reason {errorJson}");
        }

        private static async Task<Parameter> CreateOrUpdateTokenParameter(Parameter parameter, IAsyncCollector<Parameter> tokenParameters, string authToken)
        {
            if (parameter != null)
            {
                parameter.Value = authToken;
            }
            else
            {
                parameter = new Parameter()
                {
                    PartitionKey = PARAM_PARTITION_KEY,
                    RowKey = PARAM_TOKEN_ROW,
                    Value = authToken
                };

                await tokenParameters.AddAsync(parameter);
                await tokenParameters.FlushAsync();
            }

            return parameter;
        }
    }
}
