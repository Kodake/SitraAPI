using SITRA.Core.Enums;

namespace SITRA.Core.DTOs.Auths
{
    public class CreateRegistrationUserDTO
    {
        public string RucNumber { get; set; } = null!;
        public string CompanyName { get; set; } = null!;
        public string? Province { get; set; }
        public string? Municipality { get; set; }
        public string Address { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public PlanType Plan { get; set; } = PlanType.Free;
    }
}
