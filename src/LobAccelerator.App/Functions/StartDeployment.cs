#define SINGLEPASS
#undef SINGLEPASS
using LobAccelerator.App.Locators;
using LobAccelerator.App.Models;
using LobAccelerator.Library.Extensions;
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
using static LobAccelerator.App.Extensions.ConstantsExtension;


namespace LobAccelerator.App.Functions
{
    public static class StartDeployment
    {
        [FunctionName("StartDeployment")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post")]
            HttpRequestMessage req,
            [Table(PARAM_TABLE, PARAM_PARTITION_KEY, PARAM_TOKEN_ROW)]
            Parameter parameter,
            [Table(PARAM_TABLE, PARAM_PARTITION_KEY)]
            IAsyncCollector<Parameter> tokenParameters,
            [Queue(REQUEST_QUEUE)]
            CloudQueue  queue,
            ILogger log)
        {
            var accessToken = req.Headers.Authorization?.Parameter;
            ServiceLocator.BuildServiceProvider(accessToken);

            (bool valid,
            Workflow workflow,
            List<string> validationStrings) = await ValidateBodyAndAuth(req);

#if SINGLEPASS
            log.LogDebug("DEBUG: Single pass mode executing a single team/workflow entry");
            workflow.Teams = new List<TeamResource> { workflow.Teams.First() };
#endif
            log.LogInformation($"C# HTTP trigger function processing a {(!valid ? "in" : string.Empty)}valid request.");
            if (valid)
            {
                parameter = await CreateOrUpdateTokenParameter(parameter, tokenParameters, accessToken);

                foreach (var team in workflow.Teams)
                {
                    var newWorkflow = new Workflow()
                    {
                        Teams = new List<TeamResource> { team }
                    };

                    var requestBody = JsonConvert.SerializeObject(newWorkflow);
                    await queue.AddMessageAsync(new CloudQueueMessage(requestBody));
                }
                log.LogInformation($"{workflow.Teams.Count()} Teams has been schedulled for creation");
            }

            var responseString = validationStrings.ToStringLines();
            log.LogInformation("C# HTTP trigger function has completely processed a request.");

            return valid ? (ActionResult)new OkObjectResult(
                        $"{workflow.Teams.Count()} Teams are schedulled for creation")
                        : new BadRequestObjectResult($"Invalid HttpRequest, reason: {responseString}");
        }

        private static async Task<(bool, Workflow, List<string>)>
            ValidateBodyAndAuth(HttpRequestMessage req)
        {
            bool valid = true;
            var workflow = await req.Content.ReadAsAsync<Workflow>();
            List<string> validationStrings = new List<string>();

            var validator = new TeamsInputValidator();
            TeamsInputValidation configvalidation;

            foreach (var team in workflow.Teams)
            {
                if (!validator.Validate(team, out configvalidation))
                {
                    valid = false;
                }

                var verbose = validator.GetVerboseValitadion(configvalidation);
                verbose = $"Validation for {team.DisplayName}: {verbose}";
                validationStrings.Add(verbose);
            }

            var authToken = req.Headers.Authorization;
            if (string.IsNullOrWhiteSpace(authToken.Parameter))
            {
                valid = false;
                configvalidation = TeamsInputValidation.NoAuthToken;
                var verbose = validator.GetVerboseValitadion(configvalidation);
                verbose = $"Validation for Request: {verbose}";
                validationStrings.Add(verbose);
            }

            return (valid, workflow, validationStrings);
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
                parameter = new Parameter()
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