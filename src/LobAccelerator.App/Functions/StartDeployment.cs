using LobAccelerator.App.Models;
using LobAccelerator.Library.Models;
using LobAccelerator.Library.Validators;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LobAccelerator.App;
using static LobAccelerator.App.Util.GlobalSettings;

namespace LobAccelerator.App.Functions
{
    public static class StartDeployment
    {
        [FunctionName("StartDeployment")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]
            HttpRequest req,
            [Table(PARAM_TABLE, PARAM_PARTITION_KEY, PARAM_TOKEN_ROW)]
            Parameter parameter,
            [Table(PARAM_TABLE, PARAM_PARTITION_KEY)]
            IAsyncCollector<Parameter> tokenParameters,
            [Queue(REQUEST_QUEUE)]
            CloudQueue  queue,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var teamConfig = JsonConvert.DeserializeObject<TeamsJsonConfiguration>(requestBody);

            var hasToken = req.Headers.TryGetValue("Authorization", out var authTokenInfo);
            var authToken = authTokenInfo.FirstOrDefault();

            var validator = new TeamsInputValidator();
            var validated = validator.Validate(teamConfig, hasToken, out var configvalidation);
            var verbose = validator.GetVerboseValitadion(configvalidation);

            //validated implies hasToken
            if (validated)
            {
                var refreshToken = ConvertAccessTokenToRefreshToken(parameter);
                parameter = await CreateOrUpdateTokenParameter(parameter, tokenParameters, refreshToken);

                await queue.AddMessageAsync(new CloudQueueMessage(requestBody));
            }

            return validated
                ? (ActionResult)new OkObjectResult($"Team: {teamConfig.Name} is schedulled for creation")
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
