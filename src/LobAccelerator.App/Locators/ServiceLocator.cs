using LobAccelerator.Library.Configuration;
using LobAccelerator.Library.Interfaces;
using LobAccelerator.Library.Managers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;

namespace LobAccelerator.App.Locators
{
    public static class ServiceLocator
    {
        private static IServiceProvider serviceProvider;

        public static void BuildServiceProvider(ILogger logger, string accessToken)
        {
            if (serviceProvider != null) return;

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<ILogger>(logger);
            serviceCollection.AddSingleton<IConfiguration, ConfigurationSettings>();
            serviceCollection.AddSingleton<ITokenManager, TokenManager>();
            serviceCollection.AddSingleton<HttpClient, HttpClient>((sp) =>
            {
                var tokenManager = sp.GetRequiredService<ITokenManager>();
                var tokenManagerHttpMessageHandler = new TokenManagerHttpMessageHandler(tokenManager, accessToken);

                var httpClient = new HttpClient(tokenManagerHttpMessageHandler)
                {
                    BaseAddress = new Uri(sp.GetRequiredService<IConfiguration>()["GraphBaseUri"])
                };
                var desiredScopes = new string[]
                {
                "Group.ReadWrite.All",
                "User.ReadBasic.All"
                };
                httpClient.DefaultRequestHeaders.Add("X-LOBScopes", desiredScopes);

                return httpClient;
            });
            serviceCollection.AddSingleton<ITeamsManager, TeamsManager>();
            serviceCollection.AddSingleton<IOneDriveManager, OneDriveManager>();
            serviceCollection.AddSingleton<IWorkflowManager, WorkflowManager>();

            serviceProvider = serviceCollection.BuildServiceProvider();
        }

        public static TInterface GetRequiredService<TInterface>()
        {
            return serviceProvider.GetRequiredService<TInterface>();
        }
    }
}
