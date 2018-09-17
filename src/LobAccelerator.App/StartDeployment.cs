using LobAccelerator.Library.Models;
using LobAccelerator.Library.Validators;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace LobAccelerator.App
{
    public static class StartDeployment
    {
        [FunctionName("StartDeployment")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]
            HttpRequest req, ILogger log)
        {
            var validator = new TeamsInputValidator();
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var teamConfig = JsonConvert.DeserializeObject<TeamsInput>(requestBody);

            var validated = validator.Validate(teamConfig, out var configvalidation);
            var verbose = validator.GetVerboseValitadion(configvalidation);


            return validated
                ? (ActionResult)new OkObjectResult($"Team: {teamConfig.Name} is schedulled for creation")
                : new BadRequestObjectResult($"Please pass a valid TeamsConfig.json file on the request body, reason: {verbose}");
        }
    }
}
