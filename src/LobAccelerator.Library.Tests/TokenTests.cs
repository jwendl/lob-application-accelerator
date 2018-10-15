using LobAccelerator.Library.Managers;
using LobAccelerator.Library.Tests.Utils.Auth;
using LobAccelerator.Library.Tests.Utils.Configuration;
using Microsoft.Extensions.Logging.Console;
using System.Threading.Tasks;
using Xunit;

namespace LobAccelerator.Library.Tests
{
    public class TokenTests
    {
        [Fact]
        public async Task TokenManager()
        {
            //Arrange
            var configuration = new ConfigurationManager();
            var log = new ConsoleLogger("Default", null, true);
            var tokenRetriever = new TokenRetriever(configuration);
            var tokenManager = new TokenManager(configuration, log);
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
            var onBehalfOfResult = await tokenManager.GetOnBehalfOfAccessTokenAsync(
                authResult.AccessToken,
                scopes);

            //Assert
            Assert.NotNull(onBehalfOfResult);
        }
    }
}
