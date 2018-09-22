using POC.BatchRequests.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace POC.BatchRequests.Models
{
    public class BatchResponse
    {
        private readonly IEnumerable<SingleResponse> responses;

        public BatchResponse(IEnumerable<SingleResponse> responses)
        {
            this.responses = responses ?? throw new ArgumentNullException(nameof(responses));
        }

        public SingleResponse this[string requestId]
            => From(requestId);

        public SingleResponse From(string requestId)
        {
            if (string.IsNullOrWhiteSpace(requestId))
                throw new ArgumentNullException(nameof(requestId));

            return responses.First(r => r.Id == requestId);
        }
    }
}