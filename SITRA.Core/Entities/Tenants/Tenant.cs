using SITRA.Core.Enums;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Entities.Tenants
{
    public class Tenant : ITenant
    {
        public Guid TenantId { get; set; }
        public string Email { get; set; } = string.Empty;
        public PlanType Plan { get; set; } = PlanType.Free;
    }
}
