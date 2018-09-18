using System;
using System.Net.Http;

namespace LobAccelerator.Library.Factories
{
    public static class GraphClientFactory
    {
        private const string BASE_URL = "https://graph.microsoft.com/";

        public static HttpClient CreateHttpClient(string accessToken)
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(BASE_URL)
            };

            client.DefaultRequestHeaders.Add("Authorization", $"bearer {accessToken}");

            return client;
        }
    }
}
