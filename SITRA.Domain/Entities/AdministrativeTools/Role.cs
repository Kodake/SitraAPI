using SITRA.Core.Interfaces;

namespace SITRA.Core.Entities.AdministrativeTools
{
    public class Role : ITenant, IAuditableEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = null!;
        public bool IsSystemRole { get; set; } = false;
        public Guid TenantId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
        public ICollection<TenantRoleUser> TenantRoleUsers { get; set; } = new List<TenantRoleUser>();
    }
}
