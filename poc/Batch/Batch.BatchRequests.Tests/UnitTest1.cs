using POC.BatchRequests.Builders;
using POC.BatchRequests.Interfaces;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace POC.BatchRequests.Tests
{
    public class UnitTest1
    {
        [Fact]
        public async Task Test1()
        {
            var httpClient = new HttpClient();

            var batchClient = new BatchClient(httpClient);

            var content = new
            {
                city = "Redmond"
            };

            var request = new BatchRequestBuilder()
                            .Get("get1", "/me/drive/root:/{file}:/content")
                            .Delete("del1", "/groups/{id}/events/{id}").DependsOn("get1")
                            .Patch("ptc1", "/me", content)
                            .Get("get2", "/me/drive/root:/{file}:/content").DependsOn("del1", "ptc1")
                            .Build();

            var batchResponse = await batchClient.SendAsync(request);

            var operation1 = batchResponse.From("get1");
            var operation2 = batchResponse.From("get2");
            var operation3 = batchResponse["del1"];
            var operation4 = batchResponse["ptc1"];
        }
    }
}
