using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace LobAccelerator.Library.Factories
{
    public class GraphClientFactory
    {
        private const string BASE_URL = "https://graph.microsoft.com/";

        public static HttpClient CreateHttpClient(string accessToken)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(BASE_URL);
            client.DefaultRequestHeaders.Add("Authorization", $"bearer {accessToken}");
            return client;
        }
    }
}
