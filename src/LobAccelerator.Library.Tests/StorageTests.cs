using LobAccelerator.Library.Services.Interfaces;
using LobAccelerator.Library.Tests.Utils.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using System;
using System.Collections;
using System.Threading.Tasks;
using Xunit;

namespace LobAccelerator.Library.Tests
{
    public class StorageTests
    {
        private readonly string blobName = "TokenCacheBlob";
        private readonly IServiceProvider serviceProvider;

        public StorageTests()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddScoped<ILogger, ConsoleLogger>();
            serviceCollection.AddScoped<IConfiguration, ConfigurationManager>();
            serviceCollection.AddScoped<IStorageService>();
            serviceProvider = serviceCollection.BuildServiceProvider();
        }

        [Fact]
        public async Task UploadBytesToBlob()
        {
            // Arrange
            var bytes = new byte[5];
            Random random = new Random();
            random.NextBytes(bytes);

            // Act
            var storageService = serviceProvider.GetRequiredService<IStorageService>();
            var result = await storageService.UploadBlobAsync(blobName, bytes);

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
            var storageService = serviceProvider.GetRequiredService<IStorageService>();
            var uploaded = await storageService.UploadBlobAsync(blobName, bytes);
            var exists = await storageService.BlobExistsAsync(blobName);

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
            var storageService = serviceProvider.GetRequiredService<IStorageService>();
            var uploaded = await storageService.UploadBlobAsync(blobName, bytes);
            var existsAfterUploaded = await storageService.BlobExistsAsync(blobName);
            var removed = await storageService.DeleteBlobAsync(blobName);
            var existsAfterRemoved = await storageService.BlobExistsAsync(blobName);

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
            var storageService = serviceProvider.GetRequiredService<IStorageService>();
            var uploaded = await storageService.UploadBlobAsync(blobName, bytes);
            var exists = await storageService.BlobExistsAsync(blobName);
            if (uploaded && exists)
            {
                downloadedBytes = await storageService.DownloadBlobAsync(blobName);
            }

            // Assert
            Assert.True(uploaded);
            Assert.True(((IStructuralEquatable)bytes).Equals(downloadedBytes, StructuralComparisons.StructuralEqualityComparer));
        }
    }
}