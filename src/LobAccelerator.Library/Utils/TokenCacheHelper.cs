using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using System;
using System.Security.Cryptography;

namespace LobAccelerator.Library.Utils
{
    public interface ITokenCacheHelper
    {
        TokenCache FetchUserCache();
    }

    public class TokenCacheHelper
    {
        private readonly IConfiguration configuration;

        public TokenCacheHelper(IConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// Get the user token cache
        /// </summary>
        /// <returns></returns>
        public TokenCache GetUserCache()
        {
            if (usertokenCache == null)
            {
                usertokenCache = new TokenCache();
                usertokenCache.SetBeforeAccess(BeforeAccessNotification);
                usertokenCache.SetAfterAccess(AfterAccessNotification);
            }
            return usertokenCache;
        }

        static TokenCache usertokenCache;

        private static readonly object FileLock = new object();

        // TODO prevent deadlock
        /// <summary>
        /// Risk deadlock
        /// </summary>
        /// <param name="args"></param>
        public void BeforeAccessNotification(TokenCacheNotificationArgs args)
        {
            lock (FileLock)
            {
                args.TokenCache.Deserialize(
                    StorageHelper.BlobExistsAsync(
                        configuration["StorageConnectionString"],
                        configuration["TokenCacheContainerName"],
                        configuration["TokenCacheBlobName"])
                        .GetAwaiter().GetResult() //task.wait (s)
                    ? ProtectedData.Unprotect(
                        StorageHelper.DownloadBlobAsync(
                            configuration["StorageConnectionString"],
                            configuration["TokenCacheContainerName"],
                            configuration["TokenCacheBlobName"])
                        .GetAwaiter().GetResult(),
                        null,
                        DataProtectionScope.CurrentUser)
                    : null);
            }
        }

        public void AfterAccessNotification(TokenCacheNotificationArgs args)
        {
            // if the access operation resulted in a cache update
            if (args.TokenCache.HasStateChanged)
            {
                lock (FileLock)
                {
                    // reflect changes in the persistent store
                    StorageHelper.UploadBlobAsync(
                        configuration["StorageConnectionString"],
                        configuration["TokenCacheContainerName"],
                        configuration["TokenCacheBlobName"],
                        ProtectedData.Protect(args.TokenCache.Serialize(),
                                                null,
                                                DataProtectionScope.CurrentUser)
                        ).GetAwaiter().GetResult();
                    // once the write operationtakes place restore the HasStateChanged bit to filse
                    args.TokenCache.HasStateChanged = false;
                }
            }
        }
    }
}
