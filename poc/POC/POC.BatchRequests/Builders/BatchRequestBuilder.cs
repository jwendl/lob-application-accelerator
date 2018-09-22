using POC.BatchRequests.Interfaces;
using POC.BatchRequests.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace POC.BatchRequests.Builders
{
    public class BatchRequestBuilder : IBatchRequestBuilder
    {
        private readonly IList<SingleRequest> requests;

        public BatchRequestBuilder()
        {
            requests = new List<SingleRequest>();
        }

        public BatchRequestBuilder(IList<SingleRequest> requests)
        {
            this.requests = requests;
        }

        public IBatchRequestBuilderWithDepends Delete(string requestId, string requestUri)
        {
            return AddNewRequest(requestId, "DELETE", requestUri);
        }

        public IBatchRequestBuilderWithDepends Get(string requestId, string requestUri)
        {
            return AddNewRequest(requestId, "GET", requestUri);
        }

        public IBatchRequestBuilderWithDepends Patch<T>(string requestId, string requestUri, T body)
        {
            return AddNewRequestWithBody(requestId, "PATCH", requestUri, body);
        }

        public IBatchRequestBuilderWithDepends Post<T>(string requestId, string requestUri, T body)
        {
            return AddNewRequestWithBody(requestId, "POST", requestUri, body);
        }

        public IBatchRequestBuilderWithDepends Put<T>(string requestId, string requestUri, T body)
        {
            return AddNewRequestWithBody(requestId, "PUT", requestUri, body);
        }

        private IBatchRequestBuilderWithDepends AddNewRequest(string requestId, string method, string requestUri)
        {
            requests.Add(new SingleRequest(requestId, method, requestUri));

            return new BatchRequestBuilderWithDepends(requests);
        }

        private IBatchRequestBuilderWithDepends AddNewRequestWithBody(string requestId, string method, string requestUri, object body)
        {
            requests.Add(new SingleRequestWithBody(requestId, method, requestUri, body));

            return new BatchRequestBuilderWithDepends(requests);
        }

        public BatchRequest Build()
        {
            return new BatchRequest()
            {
                Requests = requests
            };
        }
    }
}
