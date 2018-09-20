using LobAccelerator.Client.Extensions;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LobAccelerator.Client.Models
{
    public class LobManager
    {
        public async Task ProvisionResourcesAsync(string url, string accessToken, IEnumerable<string> files)
        {
            using (var httpClient = new HttpClient())
            {
                foreach (var file in files)
                {
                    var content = await file.GetFileContentAsync();
                    var body = new StringContent(content, Encoding.UTF8, "application/json");

                    httpClient.DefaultRequestHeaders.Add("X-Authorization", $"bearer {accessToken}");

                    var response = await httpClient.PostAsync(url, body);
                    response.EnsureSuccessStatusCode();
                }
            }
        }
    }
}
