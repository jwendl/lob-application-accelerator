using POC.BatchRequests.Models;
using System;
using System.Collections.Generic;

namespace POC.BatchRequests.Interfaces
{
    public interface IBatchRequestBuilder
    {
        IBatchRequestBuilderWithDepends Get(string requestId, string requestUri);
        IBatchRequestBuilderWithDepends Delete(string requestId, string requestUri);
        IBatchRequestBuilderWithDepends Put<T>(string requestId, string requestUri, T body);
        IBatchRequestBuilderWithDepends Patch<T>(string requestId, string requestUri, T body);
        IBatchRequestBuilderWithDepends Post<T>(string requestId, string requestUri, T body);

        BatchRequest Build();
    }
}