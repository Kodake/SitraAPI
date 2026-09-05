using SITRA.Core.Entities.Auths;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Entities.AdministrativeTools
{
    public class TenantRoleUser : IAuditableEntity
    {
        public Guid RoleId { get; set; }
        public Role Role { get; set; } = null!;

        public string UserId { get; set; } = null!;
        public User User { get; set; } = null!;

        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
