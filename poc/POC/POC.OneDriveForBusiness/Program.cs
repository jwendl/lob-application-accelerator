using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace POC.OneDriveForBusiness
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            // Input
            string clientId = "398917db-d35d-4bd9-81cf-c3ff85c60e12";
            string teamId = "9264c179-13ad-4208-bd9c-92bd91430b3d";

            // Use cases:
            string onedriveFolder = "TransferFiles/MyFolder1/MyFolder3";
            string onedriveFile = "TransferFiles/MyFolder1/MyFolder2/Onboarding.docx";
            string onedriveFileRoot = "Book.xlsx";

            string accessToken = await GetTokenViaCode(clientId);
            var httpClient = CreateHttpClient(accessToken);
            var oneDriveManager = new OneDriveManager(httpClient);

            await oneDriveManager.CopyFolderFromOneDriveToTeams(teamId, onedriveFolder);
            await oneDriveManager.CopyFileFromOneDriveToTeams(teamId, onedriveFile);
            await oneDriveManager.CopyFileFromOneDriveToTeams(teamId, onedriveFileRoot);

            Console.ReadLine();
        }

        private static HttpClient CreateHttpClient(string accessToken)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"bearer {accessToken}");
            return httpClient;
        }

        private static async Task<string> GetTokenViaCode(string clientId)
        {
            var resource = "https://graph.microsoft.com";
            var ctx = new AuthenticationContext("https://login.microsoftonline.com/common");

            DeviceCodeResult codeResult = await ctx.AcquireDeviceCodeAsync(resource, clientId);
            Console.WriteLine("You need to sign in.");
            Console.WriteLine("Message: " + codeResult.Message);

            var token = await ctx.AcquireTokenByDeviceCodeAsync(codeResult);

            return token.AccessToken;
        }
    }
}
