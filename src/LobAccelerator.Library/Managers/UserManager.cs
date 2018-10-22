using LobAccelerator.Library.Extensions;
using LobAccelerator.Library.Managers.Interfaces;
using LobAccelerator.Library.Models.Users;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using static LobAccelerator.Library.Extensions.ConstantsExtension;

namespace LobAccelerator.Library.Managers
{
    public class UserManager
        : IUserManager
    {
        private readonly Uri baseUri;
        private readonly string apiVersion;
        private readonly ILogger logger;
        private readonly HttpClient httpClient;

        public UserManager(HttpClient httpClient, ILogger logger)
        {
            this.httpClient = httpClient;
            this.logger = logger;

            baseUri = new Uri("https://graph.microsoft.com/");
            apiVersion = TeamsApiVersion;
        }

        public async Task<UserResourceResult> CreateResourceAsync(UserResource resource)
        {
            // Create the user
            logger.LogInformation($"Starting to create the user {resource.DisplayName}");
            var result = await CreateUserAsync(resource);
            logger.LogInformation($"Finished creating the user {resource.DisplayName}");

            // Assign a license to the user, if provided
            if (!String.IsNullOrWhiteSpace(resource.LicenseName))
            {
                logger.LogInformation($"Starting to assign license {resource.LicenseName} to user {result.DisplayName}");
                result = await AssignLicenseToUser(result.Id, resource.LicenseName);
                logger.LogInformation($"Finished assigning license {resource.LicenseName} to user {result.DisplayName}");
            }

            return new UserResourceResult()
            {
                UserResult = result
            };
        }

        public async Task<UserBody> CreateUserAsync(UserResource resource)
        {
            var userUri = new Uri(baseUri, $"{apiVersion}/users");
            var requestContent = new UserBody()
            {
                AccountEnabled = true,
                DisplayName = resource.DisplayName,
                MailNickname = resource.MailNickname,
                UserPrincipalName = resource.UserPrincipalName,
                PasswordProfile = new PasswordProfile()
                {
                    Password = resource.Password,
                    ForceChangePasswordNextSignIn = resource.ForceChangePasswordNextSignIn
                },
                UsageLocation = resource.UsageLocation
            };

            logger.LogInformation($"Creating user using {userUri} and {JsonConvert.SerializeObject(requestContent)}");
            var response = await httpClient.PostContentAsync(userUri.AbsoluteUri, requestContent);
            var responseString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<UserBody>(responseString);
        }

        // Assign a license to a particular user
        private async Task<UserBody> AssignLicenseToUser(string userId, string licenseName)
        {
            // Get license details from the given string name
            var licenses = await GetSubscribedSkus();
            var license = licenses.FirstOrDefault(sku => sku.SkuPartNumber.Equals(licenseName, StringComparison.OrdinalIgnoreCase));
            if (license == null)
            {
                throw new ArgumentException($"The license {licenseName} is not valid for this organization.");
            }

            // Ensure there are units remaining for this license
            if (license.PrepaidUnits.Enabled - license.ConsumedUnits <= 0)
            {
                throw new InvalidOperationException($"There are no remaining units for the license {licenseName}");
            }

            // Assign license to user via Graph
            var uri = new Uri(baseUri, $"{apiVersion}/users/{userId}/assignLicense");
            var requestContent = new AssignLicenseBody
            {
                AddLicenses = new List<AssignedLicense>()
                {
                    new AssignedLicense
                    {
                        SkuId = license.SkuId,
                        DisabledPlans = new List<string>()
                    }
                }
            };
            logger.LogInformation($"Assigning license with ID {license.SkuId} to user with ID {userId}");
            var response = await httpClient.PostContentAsync(uri.AbsoluteUri, requestContent);
            var responseString = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<UserBody>(responseString);
        }

        // Get all subscriptions for this organization via Graph
        private async Task<IEnumerable<SubscribedSku>> GetSubscribedSkus()
        {
            var uri = new Uri(baseUri, $"{apiVersion}/subscribedSkus");
            var response = await httpClient.GetContentAsync(uri.AbsoluteUri);
            var responseString = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<SubscribedSkusResponse>(responseString).Value;
        }

        private class SubscribedSkusResponse
        {
            [JsonProperty("value")]
            public IEnumerable<SubscribedSku> Value { get; set; }
        }

        private class AssignLicenseBody
        {
            [JsonProperty("addLicenses")]
            public IEnumerable<AssignedLicense> AddLicenses { get; set; }

            [JsonProperty("removeLicenses")]
            public IEnumerable<string> RemoveLicenses { get; set; }
        }

        private class AssignedLicense
        {
            [JsonProperty("disabledPlans")]
            public IEnumerable<string> DisabledPlans { get; set; }

            [JsonProperty("skuId")]
            public string SkuId { get; set; }
        }
    }
}
