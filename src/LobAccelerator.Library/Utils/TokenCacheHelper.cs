using Microsoft.Identity.Client;
using System;
using System.Security.Cryptography;

namespace LobAccelerator.Library.Utils
{
    static class TokenCacheHelper
    {
        /// <summary>
        /// Get the user token cache
        /// </summary>
        /// <returns></returns>
        public static TokenCache GetUserCache()
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
        public static void BeforeAccessNotification(TokenCacheNotificationArgs args)
        {
            lock (FileLock)
            {
                args.TokenCache.Deserialize(
                    StorageHelper.BlobExistsAsync(
                        Environment.GetEnvironmentVariable("StorageConnectionString"),
                        Environment.GetEnvironmentVariable("TokenCacheContainerName"),
                        Environment.GetEnvironmentVariable("TokenCacheBlobName"))
                        .GetAwaiter().GetResult() //task.wait (s)
                    ? ProtectedData.Unprotect(
                        StorageHelper.DownloadBlobAsync(
                            Environment.GetEnvironmentVariable("StorageConnectionString"),
                            Environment.GetEnvironmentVariable("TokenCacheContainerName"),
                            Environment.GetEnvironmentVariable("TokenCacheBlobName"))
                        .GetAwaiter().GetResult(),
                        null,
                        DataProtectionScope.CurrentUser)
                    : null);
            }
        }

        public static void AfterAccessNotification(TokenCacheNotificationArgs args)
        {
            // if the access operation resulted in a cache update
            if (args.TokenCache.HasStateChanged)
            {
                lock (FileLock)
                {
                    // reflect changes in the persistent store
                    StorageHelper.UploadBlobAsync(
                        Environment.GetEnvironmentVariable("StorageConnectionString"),
                        Environment.GetEnvironmentVariable("TokenCacheContainerName"),
                        Environment.GetEnvironmentVariable("TokenCacheBlobName"),
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
