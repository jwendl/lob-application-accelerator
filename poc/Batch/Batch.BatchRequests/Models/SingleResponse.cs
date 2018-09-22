using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace POC.BatchRequests.Models
{
    [JsonObject("response")]
    public class SingleResponse
    {
        [JsonProperty("id")]
        public string Id { get; }
        [JsonProperty("status")]
        public string Status { get; }
        [JsonProperty("body")]
        public object Body { get; }
    }
}
