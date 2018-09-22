using Newtonsoft.Json;
using POC.BatchRequests.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace POC.BatchRequests.Models
{
    public class BatchRequest
    {
        [JsonProperty("requests")]
        public IEnumerable<SingleRequest> Requests { get; set; }
    }
}
