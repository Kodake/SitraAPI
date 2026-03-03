using System;

namespace SITRA.Core.DTOs.Auths
{
    public class CreateAssociatedUserDTO
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;

        public string? PhoneNumber { get; set; }

        public Guid RoleId { get; set; }
    }
}
