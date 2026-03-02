using Microsoft.AspNetCore.Identity;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Entities.Auths
{
    public class User : IdentityUser, ITenant
    {
        public string? CompanyName { get; set; }
        public string? RucNumber { get; set; }
        public string? Address { get; set; }
        public Guid TenantId { get; set; }
        public Language Language { get; set; } = Language.ES;

        // Parent-Child Relationship (Secondary Users)
        public string? ParentUserId { get; set; }
        public virtual User? ParentUser { get; set; }
        public virtual ICollection<User> SubUsers { get; set; } = new List<User>();

        // Invitation Flow
        public string? InvitationToken { get; set; }
        public DateTimeOffset? InvitationExpiry { get; set; }
    }
}

