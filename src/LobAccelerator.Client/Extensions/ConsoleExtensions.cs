using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Threading.Tasks;

namespace LobAccelerator.Client.Extensions
{
    public static class ConsoleExtensions
    {
        public static void DisplayError(Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Something went wrong!");
            Console.WriteLine($"Message: {ex.Message}");
        }

        public static void DisplayInfoMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(message);
        }

        public static void DisplaySuccessMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
        }

        public static async Task<string> GetTokenByCode(string resource, string clientId)
        {
            var ctx = new AuthenticationContext("https://login.microsoftonline.com/common");
            var deviceCodeResult = await ctx.AcquireDeviceCodeAsync(resource, clientId);

            Console.ResetColor();
            Console.WriteLine("You need to sign in.");
            Console.WriteLine("Message: " + deviceCodeResult.Message);

            var authResult = await ctx.AcquireTokenByDeviceCodeAsync(deviceCodeResult);

            return authResult.AccessToken;
        }
    }
}
