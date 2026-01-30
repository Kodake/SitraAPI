using Microsoft.Extensions.Caching.Memory;

namespace SITRA.API.Middlewares
{
    public class IpBlockingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;
        private readonly ILogger<IpBlockingMiddleware> _logger;
        private readonly int _maxFailedAttempts;
        private readonly int _blockDurationMinutes;

        public IpBlockingMiddleware(RequestDelegate next, IMemoryCache cache, ILogger<IpBlockingMiddleware> logger, IConfiguration configuration)
        {
            _next = next;
            _cache = cache;
            _logger = logger;
            _maxFailedAttempts = configuration.GetValue<int>("IpBlocking:MaxFailedAttempts", 5);
            _blockDurationMinutes = configuration.GetValue<int>("IpBlocking:BlockDurationMinutes", 1);
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
            if (_cache.TryGetValue(blockedKey, out _))
            {
                _logger.LogWarning($"Blocked IP {ipAddress} attempted to access.");
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Your IP has been temporarily blocked due to multiple failed login attempts.");
                return;
            }

            await _next(context);

            if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
            {
                var attemptsKey = $"ATTEMPTS_{ipAddress}";
                var attempts = _cache.GetOrCreate(attemptsKey, entry =>
                {
                    entry.SlidingExpiration = TimeSpan.FromMinutes(1); // Reset count if no activity for 10 mins
                    return 0;
                });

                attempts++;
                _cache.Set(attemptsKey, attempts);

                _logger.LogWarning($"IP {ipAddress} failed authentication. Attempt {attempts}/{_maxFailedAttempts}.");

                if (attempts >= _maxFailedAttempts)
                {
                    _logger.LogError($"Blocking IP {ipAddress} for {_blockDurationMinutes} minutes due to excessive failed attempts.");
                    var blockOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_blockDurationMinutes)
                    };
                    _cache.Set(blockedKey, true, blockOptions);
                    _cache.Remove(attemptsKey); // Clear attempts so they start fresh after block expires
                }
            }
        }
    }
}
