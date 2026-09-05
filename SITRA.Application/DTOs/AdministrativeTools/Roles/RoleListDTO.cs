namespace SITRA.Core.DTOs.AdministrativeTools.Roles
{
    public class RoleListDTO
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public bool IsSystemRole { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public List<string> Permissions { get; set; } = new List<string>();
    }
}
