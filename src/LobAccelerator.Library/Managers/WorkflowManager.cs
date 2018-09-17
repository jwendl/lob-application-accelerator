using LobAccelerator.Library.Factories;
using LobAccelerator.Library.Interfaces;
using LobAccelerator.Library.Managers;
using LobAccelerator.Library.Models;
using LobAccelerator.Library.Models.Common;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace LobAccelerator.Manager.Library
{
    public class WorkflowManager : IWorkflowManager
    {
        private readonly ITeamsManager teamsManager;

        public WorkflowManager(string accessToken)
        {
            var httpClient = GraphClientFactory.CreateHttpClient(accessToken);
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

        public async Task<Result> CreateResourceAsync(Workflow resource)
        {
            var result = new Result();

            foreach (var team in resource.Teams)
                await teamsManager.CreateResourceAsync(team);

            return result;
        }
    }
}
