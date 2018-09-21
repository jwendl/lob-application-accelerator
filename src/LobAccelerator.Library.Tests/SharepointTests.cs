using LobAccelerator.Library.Managers;
using LobAccelerator.Library.Models.SharePoint.Collections;
using LobAccelerator.Library.Tests.Utils.Auth;
using LobAccelerator.Library.Tests.Utils.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Threading.Tasks;
using Xunit;

namespace LobAccelerator.Library.Tests
{
    public class SharepointTests
    {
        [Fact]
        public async Task AddSiteCollection()
        {
            // Arrange
            var configuration = Substitute.For<IConfiguration>();
            var log = Substitute.For<ILogger>();
            var tokenRetriever = new TokenRetriever(configuration);
            var tokenManager = new TokenManager(configuration, log);
            var scopes = new string[] {
                $"api://{configuration["ClientId"]}/access_as_user"
            };

            var siteCollection = new SiteCollection()
            {
                Url = $"https://{configuration["SharePointTenantPrefix"]}.sharepoint.com/sites/rztest123",
                Owner = $"{configuration["Username"]}",
                Title = "Test123",
                Template = "STS#0",
                StorageMaximumLevel = 100,
                UserCodeMaximumLevel = 300
            };

            // Act
            var uri = await tokenManager.GetAuthUriAsync(scopes);
            var authCode = await tokenRetriever.GetAuthCodeByMsalUriAsync(uri);
            var authResult = await tokenManager.GetAccessTokenFromCodeAsync(authCode, scopes);
            //var sharepointManager = new SharePointManager(configuration, tokenManager, authResult.AccessToken);
            //var result = await sharepointManager.CreateSiteCollectionAsync(siteCollection);

            //// Assert
            //Assert.NotNull(result);
            //Assert.False(result.HasError);
        }
    }
}
