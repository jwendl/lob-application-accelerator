using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace LobAccelerator.Library.Managers
{
    public class TokenManagerHttpMessageHandler : DelegatingHandler
    {
        private ITokenManager tokenManager;
        private string accessToken;

        public TokenManagerHttpMessageHandler(ITokenManager tokenManager, string accessToken)
        {
            this.tokenManager = tokenManager;
            this.accessToken = accessToken;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var scopes = request.Headers.GetValues("X-TMScopes");
            var authResult = await this.tokenManager.GetAccessTokenAsync(this.accessToken, scopes);
            if(authResult != null)
            {
                request.Headers.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authResult.AccessToken);
            }
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
