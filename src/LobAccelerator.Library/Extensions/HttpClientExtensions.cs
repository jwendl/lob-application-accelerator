using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LobAccelerator.Library.Extensions
{
    public static class HttpClientExtensions
    {
        public static async Task<HttpResponseMessage> PostContentAsync
            (this HttpClient httpClient, string url, object content)
        {
            var objectStr = JsonConvert.SerializeObject(content);
            var body = new StringContent(objectStr, Encoding.ASCII, "application/json");
            HttpResponseMessage  resp;
            try
            {
                 resp = await httpClient.PostAsync(url, body);

            }
            catch (Exception ex)
            {
                throw;
            }

            return resp;
        }

        public static async Task<HttpResponseMessage> GetContentAsync
           (this HttpClient httpClient, string url)
        {
            return await httpClient.GetAsync(url);
        }

        public static async Task<HttpResponseMessage> PutContentAsync
            (this HttpClient httpClient, string url, object content)
        {
            var objectStr = JsonConvert.SerializeObject(content);
            var body = new StringContent(objectStr, Encoding.ASCII, "application/json");

            return await httpClient.PutAsync(url, body);
        }
    }
}
