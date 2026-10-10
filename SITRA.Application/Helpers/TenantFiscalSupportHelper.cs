using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SITRA.Core.Entities.Auths;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Helpers
{
    public static class TenantFiscalSupportHelper
    {
        public static async Task<string> GetCurrentTenantRncAsync(
            IUnitOfWork unitOfWork,
            IHttpContextAccessor? httpContextAccessor,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var claimsUser = httpContextAccessor?.HttpContext?.User;
                if (claimsUser != null)
                {
                    var rucClaim = claimsUser.FindFirst("rucNumber")?.Value;
                    if (!string.IsNullOrWhiteSpace(rucClaim))
                    {
                        return rucClaim.Trim();
                    }

                    var tenantIdStr = claimsUser.FindFirst("tenantId")?.Value;
                    if (Guid.TryParse(tenantIdStr, out var tenantId) && tenantId != Guid.Empty)
                    {
                        var owners = await unitOfWork.Repository<User>().GetReadOnlyAsync(
                            u => u.TenantId == tenantId && u.IsOwner,
                            cancellationToken);

                        var owner = owners.FirstOrDefault();
                        if (!string.IsNullOrWhiteSpace(owner?.RucNumber))
                        {
                            return owner.RucNumber.Trim();
                        }

                        var anyUserWithRnc = (await unitOfWork.Repository<User>().GetReadOnlyAsync(
                            u => u.TenantId == tenantId && !string.IsNullOrEmpty(u.RucNumber),
                            cancellationToken)).FirstOrDefault();

                        if (!string.IsNullOrWhiteSpace(anyUserWithRnc?.RucNumber))
                        {
                            return anyUserWithRnc.RucNumber.Trim();
                        }
                    }
                }
            }
            catch
            {
                // Fallback on error
            }

            return "133002868";
        }
    }
}
