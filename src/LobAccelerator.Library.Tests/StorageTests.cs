using LobAccelerator.Library.Tests.Utils.Configuration;
using LobAccelerator.Library.Utils;
using System;
using System.Collections;
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

        [Fact]
        public async Task UploadedBytesExistInBlob()
        {
            // Arrange
            var bytes = new byte[5];
            Random random = new Random();
            random.NextBytes(bytes);

            // Act
            var uploaded = await StorageHelper.UploadBlobAsync(connectionString, containerName, blobName, bytes);
            var exists = await StorageHelper.BlobExistsAsync(connectionString, containerName, blobName);

            // Assert
            Assert.True(uploaded);
            Assert.True(exists);
        }

        [Fact]
        public async Task UploadBytesExistBeforeNotAfterDeleteBlob()
        {
            // Arrange
            var bytes = new byte[5];
            Random random = new Random();
            random.NextBytes(bytes);

            // Act
            var uploaded = await StorageHelper.UploadBlobAsync(connectionString, containerName, blobName, bytes);
            var existsAfterUploaded = await StorageHelper.BlobExistsAsync(connectionString, containerName, blobName);
            var removed = await StorageHelper.DeleteBlobAsync(connectionString, containerName, blobName);
            var existsAfterRemoved = await StorageHelper.BlobExistsAsync(connectionString, containerName, blobName);

            // Assert
            Assert.True(uploaded);
            Assert.True(existsAfterUploaded);
            Assert.True(removed);
            Assert.False(existsAfterRemoved);
        }

        [Fact]
        public async Task DownloadBlobToBytes()
        {
            // Arrange
            var bytes = new byte[5];
            var downloadedBytes = new byte[5];
            Random random = new Random();
            random.NextBytes(bytes);

            // Act
            var uploaded = await StorageHelper.UploadBlobAsync(connectionString, containerName, blobName, bytes);
            var exists = await StorageHelper.BlobExistsAsync(connectionString, containerName, blobName);
            if (uploaded && exists)
            {
                downloadedBytes = await StorageHelper.DownloadBlobAsync(connectionString, containerName, blobName);
            }

            // Assert
            Assert.True(uploaded);
            Assert.True(((IStructuralEquatable)bytes).Equals(downloadedBytes, StructuralComparisons.StructuralEqualityComparer));
        }
    }
}