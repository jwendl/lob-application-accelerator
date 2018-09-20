using LobAccelerator.Library.Managers;
using System;
using System.Net.Http;

namespace LobAccelerator.Library.Factories
{
    public static class GraphClientFactory
    {
        private const string BASE_URL = "https://graph.microsoft.com/";

        public static HttpClient CreateHttpClient(ITokenManager tokenManager, string accessToken)
        {
            var tokenManagerHttpMessageHandler =
                new TokenManagerHttpMessageHandler(tokenManager, accessToken);

            var client = new HttpClient(tokenManagerHttpMessageHandler)
            {
                BaseAddress = new Uri(BASE_URL),
            };
            


            return client;
        }
    }
}
