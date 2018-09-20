using LobAccelerator.Library.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LobAccelerator.Library.Managers
{
    public class OneDriveManager : IOneDriveManager
    {
        private readonly HttpClient httpClient;

        public OneDriveManager(HttpClient httpClient)
        {
            this.httpClient = httpClient;

            var desiredScopes = new string[]
            {
                "Group.ReadWrite.All",
                "User.ReadBasic.All"
            };
            this.httpClient.DefaultRequestHeaders.Add("X-TMScopes", desiredScopes);
        }

        public async Task CopyFileFromOneDriveToTeams(string teamId, string teamChannel, string originOnedrivePath)
        {
            string copyUrlReference = string.Empty;

            var lastSlash = originOnedrivePath.LastIndexOf('/');

            if (lastSlash > -1)
            {
                var originOnedriveFolder = originOnedrivePath.Substring(0, lastSlash);
                var originOnedriveFile = originOnedrivePath.Substring(lastSlash + 1);

                copyUrlReference = $"https://graph.microsoft.com/v1.0/me/drive/root:/{originOnedriveFolder}:/children/{originOnedriveFile}/copy";
            }
            else
            {
                copyUrlReference = $"https://graph.microsoft.com/v1.0/me/drive/root/children/{originOnedrivePath}/copy";
            }

            await CopyObjectFromOneDriveToTeams(copyUrlReference, teamId, teamChannel);
        }

        public async Task CopyFolderFromOneDriveToTeams(string teamId, string teamChannel, string originOnedriveFolder)
        {
            string copyUrlReference = $"https://graph.microsoft.com/v1.0/me/drive/root:/{originOnedriveFolder}:/copy";

            await CopyObjectFromOneDriveToTeams(copyUrlReference, teamId, teamChannel);
        }

        private async Task CopyObjectFromOneDriveToTeams(string copyUrlReference, string originTeamId, string channelName = "General")
        {
            (string originDriveId, string originFolderId) = await GetDriveandPathId(originTeamId, channelName);

            var requestBody = new
            {
                parentReference = new
                {
                    driveId = originDriveId,
                    id = originFolderId
                }
            };

            var requestBodyStr = JsonConvert.SerializeObject(requestBody);
            var body = new StringContent(requestBodyStr, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(copyUrlReference, body);
            response.EnsureSuccessStatusCode();
        }

        private async Task<(string, string)> GetDriveandPathId(string teamId, string channelName)
        {
            var url = $"https://graph.microsoft.com/beta/groups/{teamId}/drive/root/children/{channelName}";

            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var obj = JObject.Parse(await response.Content.ReadAsStringAsync());

            return
                (obj.SelectToken("parentReference.driveId").ToString(),
                obj.SelectToken("id").ToString());
        }
    }
}
