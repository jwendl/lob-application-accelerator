using LobAccelerator.Library.Extensions;
using LobAccelerator.Library.Interfaces;
using LobAccelerator.Library.Managers.Interfaces;
using LobAccelerator.Library.Models.Common;
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
        private readonly Uri _baseUri;
        private readonly string _apiVersion;
        private readonly ILogger logger;
        private readonly HttpClient httpClient;

        public UserManager(HttpClient httpClient, ILogger logger)
        {
            this.httpClient = httpClient;
            this.logger = logger;

            _baseUri = new Uri("https://graph.microsoft.com/");
            _apiVersion = TeamsApiVersion;
        }

        public async Task<IResult> CreateResourceAsync(UserResource resource)
        {
            // Create the user
            logger.LogInformation($"Starting to create the user {resource.DisplayName}");
            var userResult = await CreateUserAsync(resource);
            logger.LogInformation($"Finished creating the user {resource.DisplayName}");

            // TODO: Assign the user a license

            // Combine and return results
            var results = Result.CombineSeparateResults(userResult);
            if (results.HasError())
            {
                logger.LogError($"There was an error with the UserManager: {results.GetError()}");
            }

            return results;
        }

        public async Task<Result<UserBody>> CreateUserAsync(UserResource resource)
        {
            var result = new Result<UserBody>();
            var userUri = new Uri(_baseUri, $"{_apiVersion}/users");
            var requestContent = new UserBody
            {
                AccountEnabled = true,
                DisplayName = resource.DisplayName,
                MailNickname = resource.MailNickname,
                UserPrincipalName = resource.UserPrincipalName,
                PasswordProfile = new PasswordProfile
                {
                    Password = resource.Password,
                    ForceChangePasswordNextSignIn = resource.ForceChangePasswordNextSignIn
                },
                UsageLocation = resource.UsageLocation
            };

            logger.LogInformation($"Creating user using {userUri} and {JsonConvert.SerializeObject(requestContent)}");
            var response = await httpClient.PostContentAsync(userUri.AbsoluteUri, requestContent);
            var responseString = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                result.Value = JsonConvert.DeserializeObject<UserBody>(responseString);
                return result;
            }

            result.HasError = true;
            result.Error = response.ReasonPhrase;
            result.DetailedError = responseString;

            return result;
        }
    }
}
