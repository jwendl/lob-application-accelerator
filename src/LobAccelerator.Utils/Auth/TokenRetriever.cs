using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace LobAccelerator.TestingUtils.Auth
{
    public static class TokenRetriever
    {
        public static async Task<AzureAdToken> GetTokenByAuthorizationCodeFlow()
        {
            var authorizationCode = await GetAuthorizationCode();
            var token = await GetToken(authorizationCode);

            return token;
        }

        private static async Task<string> GetAuthorizationCode()
        {
            string username = "admin@jwazuread.onmicrosoft.com";
            string password = "LOBhack!";
            string authorizeUrl = "https://login.microsoftonline.com/common/oauth2/authorize";
            string[] parameters = {
                "response_type=code",
                "client_id=398917db-d35d-4bd9-81cf-c3ff85c60e12",
                "redirect_uri=https://localhost/",
                "resource=https://graph.microsoft.com/",
                "scope=Group.ReadWrite.All"
            };

            // Navigate to login page
            IWebDriver driver = new ChromeDriver
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
            driver.Close();

            string authorizationCode = ExtractAuthorizationCodeFromReturn(returnUrl);

            return authorizationCode;
        }

        private static async Task<AzureAdToken> GetToken(string authorizationCode)
        {
            using (var httpClient = new HttpClient())
            {
                var url = "https://login.microsoftonline.com/common/oauth2/token";

                var bodyPairs = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("code", authorizationCode),
                    new KeyValuePair<string, string>("client_id", "398917db-d35d-4bd9-81cf-c3ff85c60e12"),
                    new KeyValuePair<string, string>("client_secret", "zbcaLUJ551[(ualTOPF99:$"),
                    new KeyValuePair<string, string>("resource", "https://graph.microsoft.com/"),
                    new KeyValuePair<string, string>("redirect_uri", "https://localhost/"),
                    new KeyValuePair<string, string>("scope", "Group.ReadWrite.All")
                };

                var body = new FormUrlEncodedContent(bodyPairs);

                var response = await httpClient.PostAsync(url, body);
                var responseStr = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<AzureAdToken>(responseStr);
            }
        }

        private static string ExtractAuthorizationCodeFromReturn(string returnUrl)
        {
            var regex = new Regex("code=(.*)&session_state=");
            var match = regex.Match(returnUrl);

            return match.Groups[1].Value;
        }

        private static string GetEndpointWithQueryParameters(string url, string[] parameters)
        {
            var query = parameters.Aggregate((first, second) => $"{first}&{second}");
            return $"{url}?{query}";
        }
    }
}
