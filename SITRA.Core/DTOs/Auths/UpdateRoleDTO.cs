namespace SITRA.Core.DTOs.Auths
{
    public class UpdateRoleDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
    }
}
