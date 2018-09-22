using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POC.BatchRequests.Models
{
    [JsonObject("request")]
    public class SingleRequest
    {
        [JsonProperty("id")]
        public string Id { get; }

        [JsonProperty("method")]
        public string Method { get; }

        [JsonProperty("url")]
        public string Url { get; }

        [JsonProperty("dependsOn")]
        public List<string> DependsOnList { get; private set; }

        public SingleRequest(string id, string method, string url)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentNullException(nameof(id));
            if (string.IsNullOrWhiteSpace(method)) throw new ArgumentNullException(nameof(method));
            if (string.IsNullOrWhiteSpace(url)) throw new ArgumentNullException(nameof(url));

            Id = id;
            Method = method;
            Url = url;

            DependsOnList = new List<string>();
        }

        public SingleRequest DependsOn(params string[] requestsIds)
        {
            ValidateDependentRequests(requestsIds);

            DependsOnList.AddRange(requestsIds);

            return this;
        }

        private void ValidateDependentRequests(string[] requestsIds)
        {
            if (requestsIds == null)
                throw new ArgumentNullException(nameof(requestsIds));

            if (requestsIds.Length == 0)
                throw new ArgumentException("At least one dependent request should be passed.", nameof(requestsIds));

            if (requestsIds.All(r => !string.IsNullOrWhiteSpace(r)))
                throw new ArgumentNullException(nameof(requestsIds), "At least one dependent request is null or white space");
        }
    }
}
