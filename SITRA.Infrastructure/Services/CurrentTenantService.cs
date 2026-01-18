using SITRA.Core.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace SITRA.Infrastructure.Services
{
    public class CurrentTenantService : ICurrentTenantService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentTenantService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? TenantId
        {
            get
            {
                var user = _httpContextAccessor.HttpContext?.User;
                if (user == null)
                {
                    Console.WriteLine("[CurrentTenantService] HttpContext.User is null");
                    return null;
                }

                Console.WriteLine("[CurrentTenantService] Claims available:");
                foreach (var claim in user.Claims)
                {
                    Console.WriteLine($" - {claim.Type}: {claim.Value}");
                }

                var tenantIdClaim = user.FindFirst("tenantId")?.Value;
                Console.WriteLine($"[CurrentTenantService] Found tenantId claim: {tenantIdClaim}");

                if (Guid.TryParse(tenantIdClaim, out var tenantId))
                {
                    return tenantId;
                }
                
                Console.WriteLine("[CurrentTenantService] Failed to parse tenant ID or not found.");
                return null;
            }
        }
    }
}
