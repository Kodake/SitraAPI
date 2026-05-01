namespace SITRA.Core.DTOs.Auths
{
    public class UserDTO
    {
        public string Id { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string RucNumber { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Province { get; set; } = null!;
        public string Municipality { get; set; } = null!;
        public string? CompanyName { get; set; }
        public string? ParentUserId { get; set; }
        public string? RoleId { get; set; }
    }
}
