using LobAccelerator.Library.Managers;
using LobAccelerator.Library.Managers.Interfaces;
using LobAccelerator.Library.Services;
using LobAccelerator.Library.Services.Interfaces;
using LobAccelerator.Library.Tests.Utils.Auth;
using LobAccelerator.Library.Tests.Utils.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using System;
using System.Threading.Tasks;
using Xunit;

namespace LobAccelerator.Library.Tests
{
    public class TokenTests
    {
        private readonly IServiceProvider serviceProvider;

        public TokenTests()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddScoped<ILogger>((sp) => new ConsoleLogger("test", (s, ll) => true, true));
            serviceCollection.AddScoped<IConfiguration, ConfigurationManager>();
            serviceCollection.AddScoped<IStorageService, StorageService>();
            serviceCollection.AddScoped<ITokenCacheService, TokenCacheService>();
            serviceCollection.AddScoped<ITokenRetriever, TokenRetriever>();
            serviceCollection.AddScoped<ITokenManager, TokenManager>();
            serviceProvider = serviceCollection.BuildServiceProvider();
        }

        [Fact]
        public async Task TokenManager()
        {
            //Arrange
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var tokenManager = serviceProvider.GetRequiredService<ITokenManager>();
            var tokenRetriever = serviceProvider.GetRequiredService<ITokenRetriever>();

            var scopes = new string[] {
                $"api://{configuration["ClientId"]}/access_as_user"
            };

            //Act
            var uri = await tokenManager.GetAuthUriAsync(scopes);
            var authCode = await tokenRetriever.GetAuthCodeByMsalUriAsync(uri);
            var authResult = await tokenManager.GetAccessTokenFromCodeAsync(authCode, scopes);

            scopes = new string[] {
                "Group.ReadWrite.All",
            };

            var onBehalfOfResult = await tokenManager.GetOnBehalfOfAccessTokenAsync(authResult.AccessToken, scopes);

            //Assert
            Assert.NotNull(onBehalfOfResult);
        }
    }
}
