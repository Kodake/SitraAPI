using Microsoft.AspNetCore.Identity;

namespace BackEnd.Entities.Auths
{
    public class User : IdentityUser
    {
        public string CompanyName { get; set; } = null!;
        public string RucNumber { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string TenantId { get; set; } = null!;
    }
}
