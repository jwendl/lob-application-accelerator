using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SharepointConsoleApp.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SharepointConsoleApp.Utils.Auth
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

        private Dictionary<string, AzureAdToken> TokenCaching { get; set; }

        public TokenRetriever(IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            tenantId = configuration["AzureAd:TenantId"];
            username = configuration["AzureAd:Username"];
            password = configuration["AzureAd:Password"];
            clientId = configuration["AzureAd:ClientId"];
            clientSecret = configuration["AzureAd:ClientSecret"];
            redirectUri = configuration["AzureAd:RedirectUri"];
            resource = configuration["AzureAd:Resource"];

            TokenCaching = new Dictionary<string, AzureAdToken>();
        }

        public async Task<AzureAdToken> GetTokenByAuthorizationCodeFlowAsync(params string[] desiredScopes)
        {
            ValidateScopes(desiredScopes);
            
            var scopes = ConcatScopes(desiredScopes);

            if (!TokenCaching.ContainsKey(scopes))
            {
                var authorizationCode = await GetAuthorizationCode();
                var token = await GetToken(authorizationCode, scopes);

                TokenCaching.Add(scopes, token);
            }

            return TokenCaching[scopes];
        }

        private void ValidateScopes(string[] desiredScopes)
        {
            if (!desiredScopes.Any())
            {
                throw new ArgumentNullException(nameof(desiredScopes), 
                    "Choose at least one scope. eg: Group.ReadWrite.All");
            }
        }

        private async Task<string> GetAuthorizationCode()
        {
            string authorizeUrl = $"https://login.microsoftonline.com/organizations/oauth2/v2.0/authorize";

            string[] parameters = {
                "response_type=code",
                $"client_id={clientId}",
                $"redirect_uri={redirectUri}",
                //$"resource={resource}",
                $"response_mode=query",
                $"scope=https://jwazuread.sharepoint.com/sites.readwrite.all"
            };

            string returnUrl = await LoginAndGetAuthCode(authorizeUrl, parameters);

            return ExtractAuthorizationCodeFromReturn(returnUrl);
        }

        private async Task<string> LoginAndGetAuthCode(string authorizeUrl, string[] parameters)
        {
            var url = GetEndpointWithQueryParameters(authorizeUrl, parameters);
            // Navigate to login page
            IWebDriver driver = new ChromeDriver(@"Loader")
            {
                Url = url
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

        private async Task<AzureAdToken> GetToken(string authorizationCode, string scopes)
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
                    new KeyValuePair<string, string>("response_mode", "query"),
                    //new KeyValuePair<string, string>("resource", resource),
                    new KeyValuePair<string, string>("redirect_uri", redirectUri),
                    new KeyValuePair<string, string>("scope", "https://jwazuread.sharepoint.com/sites.readwrite.all")
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
