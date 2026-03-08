using Microsoft.AspNetCore.Identity;

namespace SITRA.Core.DTOs.Auths
{
    public class RegistrationDTO
    {
        public IdentityResult IdentityResult { get; set; } = null!;
        public string? UserId { get; set; }
        public Guid? AdminRoleId { get; set; }
    }
}
