using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace LobAccelerator.Utils.Auth
{
    public class TokenRetriever
    {
        public static async Task<string> GetAccessTokenByAuthorizationCode()
        {
            var authorizationCode = await GetAuthorizationCode();
            var accessToken = await GetAccessToken(authorizationCode);

            return accessToken;
        }

        private static async Task<string> GetAuthorizationCode()
        {
            IWebDriver driver = new ChromeDriver();

            string authorizeUrl = "https://login.microsoftonline.com/common/oauth2/authorize";

            string[] parameters = {
                "response_type=code",
                "client_id=398917db-d35d-4bd9-81cf-c3ff85c60e12",
                "redirect_uri=https://localhost/",
                "resource=https://graph.microsoft.com/",
                "scope=Group.ReadWrite.All"
            };

            driver.Url = GetEndpointWithQueryParameters(authorizeUrl, parameters);

            await Task.Delay(1000);

            IWebElement emailElement = driver.FindElement(By.Name("loginfmt"));
            emailElement.SendKeys("admin@jwazuread.onmicrosoft.com");

            IWebElement firstButtonElement = driver.FindElement(By.ClassName("btn-primary"));
            firstButtonElement.Click();

            await Task.Delay(1000);

            IWebElement passwordElement = driver.FindElement(By.Name("passwd"));
            passwordElement.SendKeys("LOBhack!");

            IWebElement secondButtonElement = driver.FindElement(By.ClassName("btn-primary"));
            secondButtonElement.Click();

            await Task.Delay(1000);

            IWebElement confirmationButtonElement = driver.FindElement(By.ClassName("btn-primary"));
            confirmationButtonElement.Click();

            var regex = new Regex("code=(.*)&session_state=");
            var match = regex.Match(driver.Url);

            var authorizationCode = match.Groups[1].Value;

            driver.Close();

            return authorizationCode;
        }

        private static async Task<string> GetAccessToken(string authorizationCode)
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
                var responseContent = await response.Content.ReadAsStringAsync();
                var responseObj = JObject.Parse(responseContent);

                var accessToken = responseObj["access_token"];

                return accessToken.ToString();
            }
        }

        private static string GetEndpointWithQueryParameters(string url, string[] parameters)
        {
            var query = parameters.Aggregate((first, second) => $"{first}&{second}");
            return $"{url}?{query}";
        }
    }
}
