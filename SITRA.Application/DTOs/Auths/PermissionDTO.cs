namespace SITRA.Core.DTOs.Auths
{
    public class PermissionDTO
    {
        public Guid Id { get; set; }
        public string Resource { get; set; } = null!;
        public string Action { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public string? Description { get; set; }
        public string Module { get; set; } = null!;
        public string PermissionString { get; set; } = null!;
    }
}
