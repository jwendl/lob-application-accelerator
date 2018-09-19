using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace POC.OneDriveForBusiness
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            string resource = "https://graph.microsoft.com";
            string clientId = "398917db-d35d-4bd9-81cf-c3ff85c60e12";
            string onedriveFolder = "TransferFiles/MyFolder1/MyFolder2";

            string accessToken = await GetTokenViaCode(resource, clientId);

            HttpClient httpClient = CreateHttpClient(accessToken);

            var folderId = await GetFolderId(httpClient, onedriveFolder);
            Console.WriteLine(folderId);

            Console.ReadLine();
        }

        static HttpClient CreateHttpClient(string accessToken)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"bearer {accessToken}");
            return httpClient;
        }

        static async Task<string> GetTokenViaCode(string resource, string clientId)
        {
            var ctx = new AuthenticationContext("https://login.microsoftonline.com/common");

            DeviceCodeResult codeResult = await ctx.AcquireDeviceCodeAsync(resource, clientId);
            Console.WriteLine("You need to sign in.");
            Console.WriteLine("Message: " + codeResult.Message);

            var token = await ctx.AcquireTokenByDeviceCodeAsync(codeResult);
            return token.AccessToken;
        }

        static async Task<string> GetFolderId(HttpClient httpClient, string onedriveFolder)
        {
            string url = $"https://graph.microsoft.com/v1.0/me/drive/root:/{onedriveFolder}:?$select=id";

            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var responseStr = await response.Content.ReadAsStringAsync();
            return JObject.Parse(responseStr)["id"].Value<string>();
        }

        //static async Task CopyingFilesFromOneDriveToTeams(string url, string accessToken, IEnumerable<string> files)
        //{
        //    using (var httpClient = new HttpClient())
        //    {
        //        var body = new StringContent(content, Encoding.UTF8, "application/json");

        //        httpClient.DefaultRequestHeaders.Add("Authorization", $"bearer {accessToken}");

        //        var response = await httpClient.PostAsync(url, body);
        //        response.EnsureSuccessStatusCode();
        //    }
        //}
    }
}
