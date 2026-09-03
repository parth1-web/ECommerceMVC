using System.Net.Http.Headers;
using System.Security.Claims;

namespace ECommerceMVC.Handlers
{
    public class JwtAuthorizationHandler
        : DelegatingHandler
    {
        private readonly IHttpContextAccessor
            _httpContextAccessor;

        public JwtAuthorizationHandler(
            IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor =
                httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage>
            SendAsync(
                HttpRequestMessage request,
                CancellationToken cancellationToken)
        {
            var httpContext =
                _httpContextAccessor.HttpContext;

            if (httpContext != null)
            {
                // ==================================================
                // FIRST: TRY SESSION
                // ==================================================

                var token =
                    httpContext.Session.GetString(
                        "AccessToken");

                // ==================================================
                // FALLBACK: TRY AUTHENTICATION CLAIM
                // ==================================================

                if (string.IsNullOrWhiteSpace(token))
                {
                    token =
                        httpContext.User
                            .FindFirst("AccessToken")
                            ?.Value;

                    // Restore Session if token exists in cookie claim.
                    if (!string.IsNullOrWhiteSpace(token))
                    {
                        httpContext.Session.SetString(
                            "AccessToken",
                            token);
                    }
                }

                // ==================================================
                // ATTACH JWT
                // ==================================================

                if (!string.IsNullOrWhiteSpace(token))
                {
                    request.Headers.Authorization =
                        new AuthenticationHeaderValue(
                            "Bearer",
                            token);
                }
            }

            return await base.SendAsync(
                request,
                cancellationToken);
        }
    }
}