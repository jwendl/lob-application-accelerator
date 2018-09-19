using LobAccelerator.Library.Configuration;
using LobAccelerator.Library.Interfaces;
using LobAccelerator.Library.Managers;
using LobAccelerator.Library.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

namespace LobAccelerator.App.Locators
{
    public static class ServiceLocator
    {
        private static readonly IServiceProvider serviceProvider;

        static ServiceLocator()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<HttpClient, HttpClient>();
            serviceCollection.AddSingleton<IWorkflowManager, WorkflowManager>((sp) =>
            {
                return new WorkflowManager("");
            });
            serviceCollection.AddSingleton<ITeamsManager, TeamsManager>();
            serviceCollection.AddSingleton<IConfiguration, ConfigurationManager>();
            serviceCollection.AddSingleton<ITokenManager, TokenManager>();
            serviceCollection.AddSingleton<ITokenCacheHelper, TokenCacheHelper>();

            serviceProvider = serviceCollection.BuildServiceProvider();
        }

        public static TInterface GetRequiredService<TInterface>()
        {
            return serviceProvider.GetRequiredService<TInterface>();
        }
    }
}
