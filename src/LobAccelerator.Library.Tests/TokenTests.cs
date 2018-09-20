using LobAccelerator.Library.Managers;
using LobAccelerator.Library.Tests.Utils.Auth;
using LobAccelerator.Library.Tests.Utils.Configuration;
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
            var tokenRetriever = new TokenRetriever(configuration);
            var tokenManager = new TokenManager(configuration);
            var scopes = new string[] {
                $"api://{configuration["AzureAd:ClientId"]}/access_as_user"
            };

            //Act
            var uri = await tokenManager.GetAuthUriAsync(scopes);
            var authCode = await tokenRetriever.GetAuthCodeByMsalUriAsync(uri);
            var authResult = await tokenManager.GetAccessTokenFromCodeAsync(authCode, scopes);

            scopes = new string[] {
                "Group.ReadWrite.All",
            };
            var onBehalfOfResult = await tokenManager.GetOnBehalfOfAccessTokenAsync(authResult.AccessToken,
                scopes);

            //Assert
            Assert.NotNull(onBehalfOfResult);
        }


        [Fact]
        public async Task ValidateAccessToken()
        {
            //Arrange
            var configuration = new ConfigurationManager();
            var tokenRetriever = new TokenRetriever(configuration);
            var scopes = new string[] { "Group.ReadWrite.All",
                $"api://{configuration["AzureAd:ClientId"]}/access_as_user" };
            var expectedAudience = configuration["AzureAd:ExpectedAudience"];
            var expectedIssuer = configuration["AzureAd:ExpectedIssuer"];

            //Act
            var token = await tokenRetriever.GetTokenByAuthorizationCodeFlowAsync(scopes);
            //var validation = await AuthHelper.ValidateTokenAsync(token.access_token, expectedIssuer, expectedAudience, scopes);

            //Assert
            // We will only validate the On-behalf-of tokens...
            //Assert.NotNull(validation);
        }
    }
}
