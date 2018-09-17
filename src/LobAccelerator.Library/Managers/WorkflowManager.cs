using LobAccelerator.Library.Interfaces;
using LobAccelerator.Library.Managers;
using LobAccelerator.Library.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace LobAccelerator.Manager.Library
{
    public class WorkflowManager : IWorkflowManager
    {
        private const string BASE_URL = "https://graph.microsoft.com/";
        private readonly ITeamsManager teamsManager;
        private readonly HttpClient httpClient;

        public WorkflowManager(string accessToken)
        {
            httpClient = CreateHttpClient(BASE_URL, accessToken);
            teamsManager = new TeamsManager(httpClient);
        }

        private HttpClient CreateHttpClient(string baseUrl, string accessToken)
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };

            client.DefaultRequestHeaders.Add("Authorization", $"bearer {accessToken}");

            return client;
        }

        public async Task CreateResourceAsync(Workflow resource)
        {
            foreach (var team in resource.Teams)
                await teamsManager.CreateResourceAsync(team);
        }
    }
}
