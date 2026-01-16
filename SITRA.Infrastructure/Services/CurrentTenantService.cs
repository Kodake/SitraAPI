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
                var tenantIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("tenantId")?.Value;
                return Guid.TryParse(tenantIdClaim, out var tenantId) ? tenantId : null;
            }
        }
    }
}
