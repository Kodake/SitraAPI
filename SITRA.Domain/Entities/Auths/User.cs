using Microsoft.AspNetCore.Identity;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;
using SITRA.Core.Entities.AdministrativeTools;

namespace SITRA.Core.Entities.Auths
{
    public class User : IdentityUser, ITenant
    {
        public string? CompanyName { get; set; }
        public string? RucNumber { get; set; }
        public  string? Province { get; set; }
        public string? Municipality { get; set; }
        public string? Address { get; set; }
        public Guid TenantId { get; set; }
        public bool IsOwner { get; set; }
        public bool IsElectronicTransmitter { get; set; } = false;
        public Language Language { get; set; } = Language.ES;
        public string? CertificatePassword { get; set; }
        public string? CertificateUrl { get; set; }
        public string? CompanyLogoUrl { get; set; }
        public string? ParentUserId { get; set; }
        public virtual User? ParentUser { get; set; }
        public virtual ICollection<User> SubUsers { get; set; } = new List<User>();
        public virtual ICollection<TenantRoleUser> TenantRoleUsers { get; set; } = new List<TenantRoleUser>();
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}

