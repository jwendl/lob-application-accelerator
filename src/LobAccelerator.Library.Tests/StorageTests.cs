using System;
using System.Collections.Generic;
using System.Text;
using LobAccelerator.Library.Tests.Utils.Configuration;
using LobAccelerator.Library.Utils;

namespace LobAccelerator.Library.Tests
{
    class StorageTests
    {
        private readonly ConfigurationManager configuration;
        private readonly string connectionString = "UseDevelopmentStorage=true;";
        private readonly string containerName = "TokenCacheContainer";
        private readonly string blobName = "TokenCacheBlob";

        public StorageTests()
        {
            configuration = new ConfigurationManager();
        }
        
        [Fact]
        public async Task UploadBytesToBlob()
        {
            //Arrange
            var bytes = new byte[5];
            Random random = new Random();
            random.NextBytes(bytes);

            //Act
            var result = await StorageHelper.UploadBlobAsync(connectionString, containerName, blobName, bytes);

            //Assert
            Assert.True(result);
            Assert.False(result.HasError);
        }
    }
}
