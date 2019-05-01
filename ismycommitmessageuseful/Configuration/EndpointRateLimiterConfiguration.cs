using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace ismycommitmessageuseful.Configuration
{
    public class EndpointRateLimiterConfiguration : RateLimitConfiguration
    {
        public EndpointRateLimiterConfiguration(IHttpContextAccessor httpContextAccessor,
            IOptions<IpRateLimitOptions> ipOptions,
            IOptions<ClientRateLimitOptions> clientOptions)
                : base(httpContextAccessor, ipOptions, clientOptions)
        {
        }

        public override ICounterKeyBuilder EndpointCounterKeyBuilder => new EndpointCounterKeyBuilder();
    }
}
