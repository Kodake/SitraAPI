using Microsoft.AspNetCore.Identity;
using SITRA.Core.Enums;

namespace SITRA.Core.Entities.Auths
{
    public class User : IdentityUser
    {
        public string CompanyName { get; set; } = null!;
        public string RucNumber { get; set; } = null!;
        public string Address { get; set; } = null!;
        public Guid TenantId { get; set; }
        public Language Language { get; set; } = Language.ES;
    }
}

