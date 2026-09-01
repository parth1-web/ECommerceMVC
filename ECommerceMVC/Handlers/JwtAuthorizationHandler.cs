using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace ECommerceMVC.Handlers
{
    public class JwtAuthorizationHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JwtAuthorizationHandler(
            IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext != null)
            {
                // First try to get JWT from authentication claims.
                var accessToken = httpContext.User
                    .FindFirst("AccessToken")
                    ?.Value;

                // Fallback to Session if claim is unavailable.
                if (string.IsNullOrWhiteSpace(accessToken))
                {
                    accessToken =
                        httpContext.Session.GetString("AccessToken");
                }

                if (!string.IsNullOrWhiteSpace(accessToken))
                {
                    request.Headers.Authorization =
                        new AuthenticationHeaderValue(
                            "Bearer",
                            accessToken);
                }
            }

            return await base.SendAsync(
                request,
                cancellationToken);
        }
    }
}