using LobAccelerator.Library.Interfaces;
using LobAccelerator.Library.Models.Common;
using LobAccelerator.Library.Models.SharePoint;
using LobAccelerator.Library.Models.SharePoint.Collections;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LobAccelerator.Library.Managers
{
    public class SharePointManager
        : ISharePointManager
    {
        private readonly Uri baseUri;
        private readonly HttpClient httpClient;

        public SharePointManager(Uri baseUri, HttpClient httpClient)
        {
            this.baseUri = baseUri;
            this.httpClient = httpClient;
        }

        public async Task<IResult> CreateResourceAsync(SharePointResource sharePointResource)
        {
            var siteCollectionResult = await CreateSiteCollectionAsync(sharePointResource.DisplayName);

            return Result.CombineSeparateResults(siteCollectionResult);
        }

        public async Task<Result<SiteCollection>> CreateSiteCollectionAsync(string title)
        {
            var requestUri = new Uri(baseUri, "/_api/Site/Collections");
            var formDigest = await FetchFormDigestAsync(baseUri);
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri);
            httpRequestMessage.Headers.Authorization = httpClient.DefaultRequestHeaders.Authorization;
            httpRequestMessage.Headers.Add("X-RequestDigest", formDigest);

            var requestContent = new StringContent("{ '__metadata': { 'type': 'SP.Data.AnnouncementsListItem' }, 'Title': '" + title + "'}");
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json;odata=verbose");
            httpRequestMessage.Content = requestContent;

            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var responseString = await httpResponseMessage.Content.ReadAsStringAsync();
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
    }
}
