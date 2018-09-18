using System;
using System.Collections.Generic;
using System.Text;
using LobAccelerator.Library.Tests.Utils.Configuration;

namespace LobAccelerator.Library.Tests
{
    class StorageTests
    {
        private readonly ConfigurationManager configuration;
        private readonly string containerName = "TokenCacheContainer";
        private readonly string blobName = "TokenCacheBlob";

        public TeamsTests()
        {
            configuration = new ConfigurationManager();
        }
        
        [Fact]
        public async Task BlobExistsFalse()
        {
            //Arrange
            var team = Workflow.Teams.First();
            HttpClient httpClient = await GetHttpClient();
            var teamsManager = new TeamsManager(httpClient);

            //Act
            var result = await teamsManager.CreateGroupAsync(team);

            //Assert
            Assert.False(result.HasError);
        }

    }
}
