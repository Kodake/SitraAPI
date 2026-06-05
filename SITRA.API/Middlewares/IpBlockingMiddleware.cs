using Microsoft.Extensions.Caching.Memory;

namespace SITRA.API.Middlewares
{
    public class IpBlockingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;
        private readonly ILogger<IpBlockingMiddleware> _logger;

        public IpBlockingMiddleware(RequestDelegate next, IMemoryCache cache, ILogger<IpBlockingMiddleware> logger)
        {
            _next = next;
            _cache = cache;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var ipAddress = context.Connection.RemoteIpAddress?.ToString();

            if (string.IsNullOrEmpty(ipAddress))
            {
                await _next(context);
                return;
            }

            var blockedKey = $"BLOCKED_{ipAddress}";
            if (_cache.TryGetValue(blockedKey, out int currentTier))
            {
                _logger.LogWarning($"Blocked IP {ipAddress} attempted to access.");
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                
                string userMessage = currentTier >= 3 
                    ? "Your IP has been permanently blocked due to multiple failed login attempts. Please contact the system administrator to unlock your access."
                    : "Your IP has been temporarily blocked due to multiple failed login attempts.";

                await context.Response.WriteAsync(userMessage);
                return;
            }

            await _next(context);

            // We track login failures assuming a 401 Unauthorized status code is returned
            if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
            {
                var attemptsKey = $"ATTEMPTS_{ipAddress}";
                var tierKey = $"BLOCK_TIER_{ipAddress}";

                var attempts = _cache.GetOrCreate(attemptsKey, entry =>
                {
                    entry.SlidingExpiration = TimeSpan.FromMinutes(10); // Clear attempts if no activity for 10 mins
                    return 0;
                });

                attempts++;
                _cache.Set(attemptsKey, attempts);

                _logger.LogWarning($"IP {ipAddress} failed authentication. Attempt {attempts}/4 before next block tier.");

                if (attempts >= 4)
                {
                    var tier = _cache.GetOrCreate(tierKey, entry => 0);
                    tier++;
                    
                    // Keep tier memory valid as long as we want to remember the user's history
                    var tierOptions = new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromDays(1) };
                    _cache.Set(tierKey, tier, tierOptions);

                    TimeSpan blockDuration;
                    string blockMessage;

                    if (tier == 1)
                    {
                        blockDuration = TimeSpan.FromMinutes(1);
                        blockMessage = "for 1 minute";
                    }
                    else if (tier == 2)
                    {
                        blockDuration = TimeSpan.FromMinutes(5);
                        blockMessage = "for 5 minutes";
                    }
                    else
                    {
                        blockDuration = TimeSpan.FromDays(36500); // 100 years = permanently
                        blockMessage = "permanently";
                    }

                    _logger.LogError($"Blocking IP {ipAddress} {blockMessage} due to excessive failed attempts.");
                    
                    var blockOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = blockDuration
                    };
                    
                    _cache.Set(blockedKey, tier, blockOptions);
                    _cache.Remove(attemptsKey); // Clear attempts so they start fresh for the next tier
                }
            }
        }
    }
}
