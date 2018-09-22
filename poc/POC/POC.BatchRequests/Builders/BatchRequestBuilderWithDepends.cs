using POC.BatchRequests.Interfaces;
using POC.BatchRequests.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POC.BatchRequests.Builders
{
    public class BatchRequestBuilderWithDepends : BatchRequestBuilder, IBatchRequestBuilderWithDepends
    {
        private readonly IList<SingleRequest> requests;

        public BatchRequestBuilderWithDepends(IList<SingleRequest> requests)
        {
            this.requests = requests;
        }

        public IBatchRequestBuilder DependsOn(params string[] requestsIds)
        {
            requests.Last().DependsOn(requestsIds);

            return new BatchRequestBuilder(requests);
        }
    }
}
