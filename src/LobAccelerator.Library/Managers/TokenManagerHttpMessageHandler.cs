using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LobAccelerator.Library.Managers
{
    public class TokenManagerHttpMessageHandler : DelegatingHandler
    {
        private readonly ITokenManager tokenManager;
        private readonly string accessToken;

        public TokenManagerHttpMessageHandler(ITokenManager tokenManager, string accessToken)
        {
            this.tokenManager = tokenManager;
            this.accessToken = accessToken;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var scopes = request.Headers.GetValues("X-TMScopes");
            var authResult = await tokenManager.GetOnBehalfOfAccessTokenAsync(accessToken, scopes);
            if (authResult != null)
            {
                request.Headers.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authResult.AccessToken);
            }
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
