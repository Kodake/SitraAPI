namespace SITRA.Core.DTOs.Auths
{
    public class AssignPermissionsDTO
    {
        public Guid RoleId { get; set; }
        public List<Guid> PermissionIds { get; set; } = [];
    }
}
