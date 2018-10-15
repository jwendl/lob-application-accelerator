using LobAccelerator.Library.Tests.Extensions;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LobAccelerator.Library.Tests.Utils.Auth
{
    public class TokenRetriever
    {
        private readonly string tenantId;
        private readonly string username;
        private readonly string password;
        private readonly string clientId;
        private readonly string clientSecret;
        private readonly string redirectUri;
        private readonly string resource;
        private readonly bool headlessUITestEnabled;
        private ChromeOptions desiredCapabilities;

        private Dictionary<string, AzureAdToken> TokenCaching { get; set; }

        public TokenRetriever(IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            tenantId = configuration["TenantId"];
            username = configuration["Username"];
            password = configuration["Password"];
            clientId = configuration["ClientId"];
            clientSecret = configuration["ClientSecret"];
            redirectUri = configuration["RedirectUri"];
            resource = configuration["Resource"];
            headlessUITestEnabled = bool.Parse(configuration["HeadlessUITestEnabled"]);

            desiredCapabilities = new ChromeOptions();
            desiredCapabilities.AddArgument("--ignore-ssl-errors=true");
            desiredCapabilities.AddArgument("--ssl-protocol=any");
            if(headlessUITestEnabled)
            {
                desiredCapabilities.AddArgument("--headless");
            }

            TokenCaching = new Dictionary<string, AzureAdToken>();
        }

        public async Task<AzureAdToken> GetTokenByAuthorizationCodeFlowAsync(params string[] desiredScopes)
        {
            if (desiredScopes.Count() == 0)
                throw new ArgumentNullException(nameof(desiredScopes), "Choose at least one scope. eg: Group.ReadWrite.All");

            string scopes = ConcatScopes(desiredScopes);

            if (!TokenCaching.ContainsKey(scopes))
            {
                string authorizationCode = await GetAuthorizationCodeAsync(scopes);
                AzureAdToken token = await GetTokenAsync(authorizationCode, scopes);

                TokenCaching.Add(scopes, token);
            }

            return TokenCaching[scopes];
        }

        public async Task<string> GetAuthCodeByMsalUriAsync(Uri msalUri)
        {
            var authorizationCode = await GetAuthorizationCodeByMsalUriAsync(msalUri);
            return authorizationCode;
        }

        private async Task<string> GetAuthorizationCodeAsync(string scopes)
        {
            string authorizeUrl =
                string.Format("https://login.microsoftonline.com/{0}/oauth2/authorize",
                    tenantId);

            string[] parameters = {
                "response_type=code",
                $"client_id={clientId}",
                $"redirect_uri={redirectUri}",
                $"resource={resource}",
                $"scope=api://{clientId}/access_as_user"
            };

            string returnUrl = await LoginAndGetAuthCodeAsync(authorizeUrl, parameters);

            string authorizationCode = ExtractAuthorizationCodeFromReturn(returnUrl);

            return authorizationCode;
        }

        private async Task<string> GetAuthorizationCodeByMsalUriAsync(Uri msalUri)
        {
            string returnUrl = await LoginAndGetAuthCodeByMsalUriAsync(msalUri);

            string authorizationCode = ExtractAuthorizationCodeFromReturn(returnUrl);

            return authorizationCode;
        }

        private async Task<string> LoginAndGetAuthCodeAsync(string authorizeUrl, string[] parameters)
        {
            // Navigate to login page
            IWebDriver driver = new ChromeDriver(@"Loader", desiredCapabilities)
            {
                Url = GetEndpointWithQueryParameters(authorizeUrl, parameters)
            };
            await Task.Delay(1000);

            // Login with username
            driver.SendTextToTextBox("loginfmt", username);
            driver.ClickOnButton("btn-primary");
            await Task.Delay(1000);

            // Login with password
            driver.SendTextToTextBox("passwd", password);
            driver.ClickOnButton("btn-primary");
            await Task.Delay(1000);

            // Confirms to store credentials
            driver.ClickOnButton("btn-primary");

            // Receive Authorization code
            var returnUrl = driver.Url;

            // Close browser
            driver.Close();

            return returnUrl;
        }

        private async Task<string> LoginAndGetAuthCodeByMsalUriAsync(Uri msalUri)
        {
            // Navigate to login page
            IWebDriver driver = new ChromeDriver(@"Loader", desiredCapabilities)
            {
                Url = msalUri.AbsoluteUri
            };
            await Task.Delay(1000);

            // Login with username
            driver.SendTextToTextBox("loginfmt", username);
            driver.ClickOnButton("btn-primary");
            await Task.Delay(1000);

            // Login with password
            driver.SendTextToTextBox("passwd", password);
            driver.ClickOnButton("btn-primary");
            await Task.Delay(1000);

            // Confirms to store credentials
            //driver.ClickOnButton("btn-primary");

            // Receive Authorization code
            var returnUrl = driver.Url;

            // Close browser
            driver.Close();

            return returnUrl;
        }

        private async Task<AzureAdToken> GetTokenAsync(string authorizationCode, string scopes)
        {
            string tokenUrl =
                string.Format("https://login.microsoftonline.com/{0}/oauth2/token",
                    tenantId);

            using (var httpClient = new HttpClient())
            {
                var bodyPairs = BuildBodyForTokenRequest(authorizationCode, scopes);
                var body = new FormUrlEncodedContent(bodyPairs);

                var response = await httpClient.PostAsync(tokenUrl, body);
                response.EnsureSuccessStatusCode();

                var responseStr = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<AzureAdToken>(responseStr);
            }
        }

        private List<KeyValuePair<string, string>> BuildBodyForTokenRequest
            (string authorizationCode, string scopes)
        {
            return new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("code", authorizationCode),
                    new KeyValuePair<string, string>("client_id", clientId),
                    new KeyValuePair<string, string>("client_secret", clientSecret),
                    new KeyValuePair<string, string>("resource", resource),
                    new KeyValuePair<string, string>("redirect_uri", redirectUri),
                    new KeyValuePair<string, string>("scope", scopes)
                };
        }

        #region Processing String Methods

        private string ConcatScopes(string[] scopes)
        {
            return scopes.Aggregate((s1, s2) => $"{s1},{s2}");
        }

        private string ExtractAuthorizationCodeFromReturn(string returnUrl)
        {
            var regex = new Regex("code=(.*)&session_state=");
            var match = regex.Match(returnUrl);

            return match.Groups[1].Value;
        }

        private string GetEndpointWithQueryParameters(string url, string[] parameters)
        {
            var query = parameters.Aggregate((first, second) => $"{first}&{second}");
            return $"{url}?{query}";
        }

        #endregion
    }
}
