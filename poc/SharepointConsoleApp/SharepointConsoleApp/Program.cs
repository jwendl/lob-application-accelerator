using SharepointConsoleApp.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SharepointConsoleApp
{
    public static class Program
    {
        static void Main(string[] args)
        {
            Run().Wait();

        }

        private static async Task Run()
        {
            var resource = new SharePointResource
            {
                DisplayName = "test"
            };
            
            var httpClient = new HttpClient();
            var manager = new SharePointManager(httpClient);

            var result = await manager.CreateResourceAsync(resource);
        }
    }
}
