using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace ismycommitmessageuseful.RateLimiting
{
    public class CorsIpRateLimitMiddleware : IpRateLimitMiddleware
    {
        private readonly IpRateLimitOptions _options;

        public CorsIpRateLimitMiddleware(RequestDelegate next, 
            IOptions<IpRateLimitOptions> options, 
            IRateLimitCounterStore counterStore, 
            IIpPolicyStore policyStore,
            IRateLimitConfiguration config,
            ILogger<IpRateLimitMiddleware> logger) 
            : base(next, options, counterStore, policyStore, config, logger)
        {
            _options = options.Value;
        }

        public override Task ReturnQuotaExceededResponse(HttpContext httpContext, RateLimitRule rule, string retryAfter)
        {
            string text = string.Format(_options.QuotaExceededResponse?.Content ?? _options.QuotaExceededMessage ?? "API calls quota exceeded! maximum admitted {0} per {1}.", rule.Limit, rule.Period, retryAfter);
            if (!_options.DisableRateLimitHeaders)
            {
                httpContext.Response.Headers["Retry-After"] = retryAfter;

                httpContext.Response.Headers["Access-Control-Allow-Methods"] = "GET, PUT, POST, DELETE, HEAD, OPTIONS";
                httpContext.Response.Headers["Access-Control-Allow-Origin"] = "*";
            }
            httpContext.Response.StatusCode = (_options.QuotaExceededResponse?.StatusCode ?? _options.HttpStatusCode);
            httpContext.Response.ContentType = (_options.QuotaExceededResponse?.ContentType ?? "text/plain");
            return httpContext.Response.WriteAsync(text);
        }
    }
}
