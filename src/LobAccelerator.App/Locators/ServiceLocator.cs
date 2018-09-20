using LobAccelerator.Library.Configuration;
using LobAccelerator.Library.Interfaces;
using LobAccelerator.Library.Managers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

namespace LobAccelerator.App.Locators
{
    public static class ServiceLocator
    {
        private static IServiceProvider serviceProvider;

        public static void BuildServiceProvider(string accessToken)
        {
            if (serviceProvider != null) return;

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IConfiguration, ConfigurationSettings>();
            serviceCollection.AddSingleton<ITokenManager, TokenManager>();
            serviceCollection.AddSingleton<HttpClient, HttpClient>((sp) =>
            {
                var tokenManager = sp.GetRequiredService<ITokenManager>();
                var tokenManagerHttpMessageHandler = new TokenManagerHttpMessageHandler(tokenManager, accessToken);
                return new HttpClient(tokenManagerHttpMessageHandler);
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
