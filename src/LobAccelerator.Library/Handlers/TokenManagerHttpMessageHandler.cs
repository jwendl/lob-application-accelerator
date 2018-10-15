using LobAccelerator.Library.Managers.Interfaces;
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
        private readonly ITokenManager tokenManager;
        private readonly string accessToken;
        private readonly PolicyBuilder<HttpResponseMessage> retryPolicy;

        public TokenManagerHttpMessageHandler(ITokenManager tokenManager, string accessToken)
        {
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

            return await retryPolicy
                .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(3, retryAttempt)))
                .ExecuteAsync(() => base.SendAsync(request, cancellationToken));
        }
    }
}
