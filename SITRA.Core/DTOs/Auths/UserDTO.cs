namespace SITRA.Core.DTOs.Auths
{
    public class UserDTO
    {
        public string Id { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? CompanyName { get; set; }
        public string? ParentUserId { get; set; }
    }
}
