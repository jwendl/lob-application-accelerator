using LobAccelerator.App.Models;
using LobAccelerator.Library.Models;
using LobAccelerator.Library.Validators;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LobAccelerator.App
{
    public static class StartDeployment
    {
        const string TABLE_NAME = "parameters";
        const string PARTITION_KEY = "Authorization";
        const string TOKEN_ROW = "refresh-token";

        [FunctionName("StartDeployment")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]
            HttpRequest req,
            [Table(TABLE_NAME, PARTITION_KEY, TOKEN_ROW)]
            Parameter parameter,
            [Table(TABLE_NAME, PARTITION_KEY)]
            IAsyncCollector<Parameter> tokenParameters,
            ILogger log)
        {
            var validator = new TeamsInputValidator();
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var teamConfig = JsonConvert.DeserializeObject<TeamsInput>(requestBody);

            var hasToken = req.Headers.TryGetValue("Authorization", out var authTokenInfo);
            var authToken = authTokenInfo.FirstOrDefault();
            var validated = validator.Validate(teamConfig, hasToken, out var configvalidation);
            var verbose = validator.GetVerboseValitadion(configvalidation);

            //validated implies hasToken
            if (validated)
            {
                var refreshToken = ConvertAccessTokenToRefreshToken(parameter.Value);
                parameter = await CreateOrUpdateTokenParameter(parameter, tokenParameters, refreshToken);
            }

            return validated
                ? (ActionResult)new OkObjectResult($"Team: {teamConfig.Name} is schedulled for creation")
                : new BadRequestObjectResult($"Invalid HttpRequest, reason: {verbose}");
        }

        private static string ConvertAccessTokenToRefreshToken(string value)
        {
            return "NOT IMPLEMENTED YET";
        }

        private static async Task<Parameter> CreateOrUpdateTokenParameter(Parameter parameter, IAsyncCollector<Parameter> tokenParameters, string authToken)
        {
            if (parameter != null)
            {
                parameter.Value = authToken;
            }
            else
            {
                parameter = new Parameter
                {
                    PartitionKey = PARTITION_KEY,
                    RowKey = TOKEN_ROW,
                    Value = authToken
                };

                await tokenParameters.AddAsync(parameter);
            }
            return parameter;
        }
    }
}