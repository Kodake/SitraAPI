using SITRA.Core.Interfaces;

namespace SITRA.Core.Entities.AdministrativeTools
{
    public class RolePermission : IAuditableEntity
    {
        public Guid RoleId { get; set; }
        public Role Role { get; set; } = null!;

        public Guid PermissionId { get; set; }
        public Permission Permission { get; set; } = null!;

        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
