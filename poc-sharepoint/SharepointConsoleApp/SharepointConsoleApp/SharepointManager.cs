using Microsoft.Extensions.Configuration;
using SharepointConsoleApp.Interfaces;
using SharepointConsoleApp.Models;
using SharepointConsoleApp.Models.Common;
using SharepointConsoleApp.Models.SharePoint;
using SharepointConsoleApp.Utils.Auth;
using SharepointConsoleApp.Utils.Configuration;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SharepointConsoleApp
{
    public class SharePointManager: ISharePointManager
    {
        private readonly HttpClient httpClient;
        private readonly IConfiguration _configuration;

        public SharePointManager(HttpClient httpClient)
        {
            this.httpClient = httpClient;
            _configuration = new ConfigurationManager();
        }

        public async Task<IResult> CreateResourceAsync(SharePointResource sharePointResource)
        {
            var siteCollectionResult = await CreateSiteCollectionAsync(sharePointResource.DisplayName);

            return Result.Combine(siteCollectionResult);
        }

        public async Task<Result<SiteCollection>> CreateSiteCollectionAsync(string title)
        {
            try
            {
                var requestUri = GetRequestUri();
                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri);

                var accessToken = await GetAccessTokenAsync();
                //var formDigest = await FetchFormDigestAsync(requestUri);

                httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                //httpRequestMessage.Headers.Add("X-RequestDigest", formDigest);

                var requestContent = new StringContent("{ '__metadata': { 'type': 'SP.Data.AnnouncementsListItem' }, 'Title': '" + title + "'}");

                requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json;odata=verbose");
                httpRequestMessage.Content = requestContent;

                var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    var responseString = await httpResponseMessage.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return null;
        }

        private async Task<string> FetchFormDigestAsync(Uri siteUri)
        {
            //Get the form digest value in order to write data
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, new Uri(siteUri, "/_api/contextinfo"));
            httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
            httpRequestMessage.Headers.Authorization = httpClient.DefaultRequestHeaders.Authorization;
            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

            var responseString = await httpResponseMessage.Content.ReadAsStringAsync();
            var xNamespace = "http://schemas.microsoft.com/ado/2007/08/dataservices";
            var root = XElement.Parse(responseString);
            var formDigestValue = root.Element(xNamespace + "FormDigestValue").Value;

            return formDigestValue;
        }

        private Uri GetRequestUri()
        {
            var tenantId = _configuration["AzureAd:TenantName"];
            var baseUri = new Uri($"https://{tenantId}.sharepoint.com/");

            return new Uri(baseUri, "/_api/Site/Collections");
        }

        public async Task<string> GetAccessTokenAsync()
        {
            var tokenRetriever = new TokenRetriever(new ConfigurationManager());

            var scopes = new string[] {
                "AllSites.FullControl"
            };

            var token = await tokenRetriever.GetTokenByAuthorizationCodeFlowAsync(scopes);

            return token.access_token;
        }
    }
}
