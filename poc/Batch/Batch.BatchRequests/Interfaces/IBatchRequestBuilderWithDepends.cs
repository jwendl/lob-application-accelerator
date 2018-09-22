using System;
using System.Collections.Generic;
using System.Text;

namespace POC.BatchRequests.Interfaces
{
    public interface IBatchRequestBuilderWithDepends : IBatchRequestBuilder
    {
        IBatchRequestBuilder DependsOn(params string[] requestsIds);
    }
}
