using Newtonsoft.Json;
using POC.BatchRequests.Models;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace POC.BatchRequests
{
    public class BatchClient
    {
        private HttpClient httpClient;

        public BatchClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<BatchResponse> SendAsync(BatchRequest batchRequest)
        {
            return await SendAsync(batchRequest, new CancellationToken());
        }

        public async Task<BatchResponse> SendAsync(BatchRequest batchRequest, CancellationToken cancellationToken)
        {
            var batchRequestStr = JsonConvert.SerializeObject(batchRequest);
            var content = new StringContent(batchRequestStr, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("v1.0/$batch", content, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<BatchResponse>(responseBody);
        }

    }
}
