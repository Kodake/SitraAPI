using Microsoft.AspNetCore.Http;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces;

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
                    Console.WriteLine(Utilities.TenantServiceUserNull);
                    return null;
                }

                Console.WriteLine(Utilities.TenantServiceClaimsAvailable);
                foreach (var claim in user.Claims)
                {
                    Console.WriteLine(Utilities.TenantServiceClaimInfo(claim.Type, claim.Value));
                }

                var tenantIdClaim = user.FindFirst("tenantId")?.Value;
                Console.WriteLine(Utilities.TenantServiceFoundClaim(tenantIdClaim));

                if (Guid.TryParse(tenantIdClaim, out var tenantId))
                {
                    return tenantId;
                }

                Console.WriteLine(Utilities.TenantServiceFailedToParse);
                return null;
            }
        }
    }
}
