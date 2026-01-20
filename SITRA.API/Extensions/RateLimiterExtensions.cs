using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using System;

namespace SITRA.API.Extensions
{
    public static class RateLimiterExtensions
    {
        public static IServiceCollection AddRateLimiterConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.AddPolicy("fixed", context =>
                {
                    // Default limits (Free plan or unauthenticated)
                    var rateLimitingSettings = configuration.GetSection("RateLimiting");
                    var permitLimit = rateLimitingSettings.GetValue<int>("PermitLimit", 10);
                    var window = TimeSpan.FromSeconds(rateLimitingSettings.GetValue<int>("WindowSeconds", 60));
                    var queueLimit = rateLimitingSettings.GetValue<int>("QueueLimit", 0);

                    // Check if user is authenticated and has a Tenant
                    if (context.User.Identity?.IsAuthenticated == true)
                    {
                        var tenantIdClaim = context.User.FindFirst("tenantId")?.Value;
                        if (Guid.TryParse(tenantIdClaim, out var tenantId))
                        {
                            // Resolve DbContext to get Tenant Plan
                            // Note: This is a synchronous blocking call in a customized policy, be cautious with performance.
                            // In a real high-throughput scenario, cache this heavily!
                            var dbContext = context.RequestServices.GetRequiredService<SITRA.Infrastructure.Persistence.ApplicationDbContext>();
                            var tenant = dbContext.Tenants.FirstOrDefault(t => t.TenantId == tenantId);

                            if (tenant != null)
                            {
                                switch (tenant.Plan)
                                {
                                    case SITRA.Core.Enums.PlanType.Basic:
                                        permitLimit = 100;
                                        break;
                                    case SITRA.Core.Enums.PlanType.Premium:
                                        permitLimit = 500;
                                        break;
                                    default: // Free
                                        permitLimit = 10;
                                        break;
                                }

                                return RateLimitPartition.GetFixedWindowLimiter(
                                    partitionKey: tenantId.ToString(),
                                    factory: partition => new FixedWindowRateLimiterOptions
                                    {
                                        AutoReplenishment = true,
                                        PermitLimit = permitLimit,
                                        Window = window,
                                        QueueLimit = queueLimit,
                                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                                    });
                            }
                        }
                    }

                    // Fallback to IP-based limiting for unauthenticated or unknown tenants
                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        factory: partition => new FixedWindowRateLimiterOptions
                        {
                            AutoReplenishment = true,
                            PermitLimit = permitLimit, 
                            Window = window,
                            QueueLimit = queueLimit,
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                        });
                });
            });

            return services;
        }
    }
}
