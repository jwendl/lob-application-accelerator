using LobAccelerator.Library.Managers.Interfaces;
using Microsoft.Extensions.Logging;
using Polly;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace LobAccelerator.Library.Handlers
{
    public class TokenManagerHttpMessageHandler
        : DelegatingHandler
    {
        private readonly ILogger logger;
        private readonly ITokenManager tokenManager;
        private readonly string accessToken;
        private readonly PolicyBuilder<HttpResponseMessage> retryPolicy;

        public TokenManagerHttpMessageHandler(ILogger logger, ITokenManager tokenManager, string accessToken)
        {
            this.logger = logger;
            this.tokenManager = tokenManager;
            this.accessToken = accessToken;
            retryPolicy = Policy
                    .Handle<HttpRequestException>()
                    .Or<TaskCanceledException>()
                    .OrResult<HttpResponseMessage>(x => !x.IsSuccessStatusCode);

            InnerHandler = new HttpClientHandler();
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var scopes = request.Headers.GetValues("X-LOBScopes");
            var authResult = await tokenManager.GetOnBehalfOfAccessTokenAsync(accessToken, scopes);
            if (authResult != null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authResult.AccessToken);
            }

            var httpResponseMessage = await retryPolicy
                .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(3, retryAttempt)), async (hrm, timeSpan, retryCount, context) =>
                {
                    var message = await hrm.Result.Content.ReadAsStringAsync();
                    logger.LogWarning($"Retrying request for {request.RequestUri} as it failed {retryCount + 1} time(s) so far with {hrm.Result.StatusCode}. Waiting {timeSpan} next attempt. Message: {message}.");
                })
                .ExecuteAsync(() => base.SendAsync(request, cancellationToken));

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                var message = await httpResponseMessage.Content.ReadAsStringAsync();
                logger.LogError($"The request to {request.RequestUri} failed too many times with error code {httpResponseMessage.StatusCode}. The message is: {message}.");
                throw new InvalidOperationException($"The request to {request.RequestUri} failed too many times with error code {httpResponseMessage.StatusCode}. The message is: {message}.");
            }

            return httpResponseMessage;
        }
    }
}
