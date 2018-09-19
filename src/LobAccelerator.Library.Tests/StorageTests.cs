using LobAccelerator.Library.Tests.Utils.Configuration;
using LobAccelerator.Library.Utils;
using System;
using System.Threading.Tasks;
using Xunit;

namespace LobAccelerator.Library.Tests
{
    public class StorageTests
    {
        private readonly ConfigurationManager configuration;
        private readonly string connectionString = "UseDevelopmentStorage=true;";
        private readonly string containerName = "tokencachecontainer";
        private readonly string blobName = "TokenCacheBlob";

        public StorageTests()
        {
            configuration = new ConfigurationManager();
        }

        [Fact]
        public async Task UploadBytesToBlob()
        {
            // Arrange
            var bytes = new byte[5];
            Random random = new Random();
            random.NextBytes(bytes);

            // Act
            var result = await StorageHelper.UploadBlobAsync(connectionString, containerName, blobName, bytes);

            // Assert
            Assert.True(result);
        }
    }
}
