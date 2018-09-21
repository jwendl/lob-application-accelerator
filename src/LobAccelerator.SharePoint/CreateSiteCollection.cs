using LobAccelerator.Library.Configuration;
using LobAccelerator.Library.Managers;
using LobAccelerator.Library.Models.SharePoint.Collections;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace LobAccelerator.SharePoint
{
    public static class CreateSiteCollection
    {
        [FunctionName("CreateSiteCollection")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            var configuration = new ConfigurationSettings();
            var tokenManager = new TokenManager(configuration);
            var scopes = new string[] {
                $"api://{configuration["ClientId"]}/access_as_user"
            };

            var siteCollection = new SiteCollection()
            {
                Url = $"https://{configuration["SharePointTenantPrefix"]}.sharepoint.com/sites/rztest123",
                Owner = $"{configuration["Username"]}",
                Title = "Test123",
                Template = "STS#0",
                StorageMaximumLevel = 100,
                UserCodeMaximumLevel = 300
            };

            var uri = await tokenManager.GetAuthUriAsync(scopes);
            //var authCode = await tokenRetriever.GetAuthCodeByMsalUriAsync(uri);
            var authenticationHeaderValue = req.Headers.Authorization;
            var authResult = await tokenManager.GetAccessTokenFromCodeAsync(authenticationHeaderValue.Parameter, scopes);
            var sharepointManager = new SharePointManager(configuration, tokenManager, authResult.AccessToken);
            var results = await sharepointManager.CreateSiteCollectionAsync(siteCollection);
            var httpContent = new StringContent(JsonConvert.SerializeObject(results));

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = httpContent,
            };
        }
    }
}
