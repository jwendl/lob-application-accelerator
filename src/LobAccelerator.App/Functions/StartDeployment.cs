using LobAccelerator.App.Models;
using LobAccelerator.Library.Models;
using LobAccelerator.Library.Models.Teams;
using LobAccelerator.Library.Validators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using static LobAccelerator.App.Util.GlobalSettings;

namespace LobAccelerator.App.Functions
{
    public static class StartDeployment
    {
        [FunctionName("StartDeployment")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]
            HttpRequestMessage req,
            [Table(PARAM_TABLE, PARAM_PARTITION_KEY, PARAM_TOKEN_ROW)]
            Parameter parameter,
            [Table(PARAM_TABLE, PARAM_PARTITION_KEY)]
            IAsyncCollector<Parameter> tokenParameters,
            [Queue(REQUEST_QUEUE)]
            CloudQueue  queue,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            var workflow = await req.Content.ReadAsAsync<Workflow>();
            var authToken = req.Headers.Authorization;
            var hasToken = !string.IsNullOrWhiteSpace(authToken.Parameter);

            var teamConfig = workflow.Teams.FirstOrDefault();
            var newWorkflow = new Workflow
                        {
                            Teams = new List<TeamResource> { teamConfig }
                        };
            var validator = new TeamsInputValidator();
            var validated = validator.Validate(teamConfig, hasToken, out var configvalidation);
            var verbose = validator.GetVerboseValitadion(configvalidation);

            //validated implies hasToken
            if (validated)
            {
                var refreshToken = ConvertAccessTokenToRefreshToken(parameter);
                parameter = await CreateOrUpdateTokenParameter(parameter, tokenParameters, refreshToken);

                var requestBody = JsonConvert.SerializeObject(newWorkflow);
                await queue.AddMessageAsync(new CloudQueueMessage(requestBody));
            }

            return validated
                ? (ActionResult)new OkObjectResult($"Team: {teamConfig.DisplayName} is schedulled for creation")
                : new BadRequestObjectResult($"Invalid HttpRequest, reason: {verbose}");
        }

        private static string ConvertAccessTokenToRefreshToken(Parameter acessToken)
        {
            return "NOT IMPLEMENTED YET";
        }

        private static async Task<Parameter> CreateOrUpdateTokenParameter(
            Parameter parameter,
            IAsyncCollector<Parameter> tokenParameters,
            string authToken)
        {
            if (parameter != null)
            {
                parameter.Value = authToken;
            }
            else
            {
                parameter = new Parameter
                {
                    PartitionKey = PARAM_PARTITION_KEY,
                    RowKey = PARAM_TOKEN_ROW,
                    Value = authToken
                };

                await tokenParameters.AddAsync(parameter);
            }
            return parameter;
        }
    }
}
