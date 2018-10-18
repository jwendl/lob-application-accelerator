using LobAccelerator.Library.Extensions;
using LobAccelerator.Library.Managers.Interfaces;
using LobAccelerator.Library.Models.Users;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
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
            var userResult = await CreateUserAsync(resource);
            logger.LogInformation($"Finished creating the user {resource.DisplayName}");

            // TODO: Assign the user a license

            // Combine and return results
            return new UserResourceResult()
            {
                UserResult = userResult,
            };
        }

        public async Task<UserBody> CreateUserAsync(UserResource resource)
        {
            var result = new UserBody();
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

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"There was an error with creating a user ({response.StatusCode}): {responseString}");
            }

            return JsonConvert.DeserializeObject<UserBody>(responseString);
        }
    }
}
