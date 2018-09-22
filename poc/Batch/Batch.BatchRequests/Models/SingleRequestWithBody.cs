using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace POC.BatchRequests.Models
{
    [JsonObject("request")]
    public class SingleRequestWithBody : SingleRequest
    {
        [JsonProperty("body")]
        public object Body { get; }

        public SingleRequestWithBody(string id, string method, string url, object body)
            : base(id, method, url)
        {
            if (body == null) throw new ArgumentNullException(nameof(body));

            Body = body;
        }
    }
}
