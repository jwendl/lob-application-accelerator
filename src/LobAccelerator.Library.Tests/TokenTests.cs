using LobAccelerator.Library.Tests.Utils.Auth;
using LobAccelerator.Library.Tests.Utils.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace LobAccelerator.Library.Tests
{
    public class TokenTests
    {
        [Fact]
        public async Task RetrieveTokenByAuthorizationCodeFlow()
        {
            //Arrange
            var configuration = new ConfigurationManager();
            var tokenRetriever = new TokenRetriever(configuration);
            var scopes = new string[] { "Group.ReadWrite.All" };

            //Act
            var token = await tokenRetriever.GetTokenByAuthorizationCodeFlowAsync(scopes);
            var returnedScopes = token.scope.Split(' ');

            //Assert
            Assert.NotNull(token);
            Assert.NotNull(token.access_token);
            Assert.NotNull(token.refresh_token);
            Assert.NotNull(token.id_token);
            Assert.Equal("Bearer", token.token_type);
            Assert.True(int.Parse(token.expires_in) > 3500);
            Assert.NotEmpty(returnedScopes);
            Assert.True(returnedScopes.Intersect(scopes).Count() == scopes.Count());
            Assert.Equal(configuration["AzureAd:Resource"], token.resource);
        }
    }
}
