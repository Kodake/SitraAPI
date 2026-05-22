using Microsoft.AspNetCore.Http;

namespace SITRA.Core.DTOs.Auths
{
    public class UpdateProfileNameDTO
    {
        public string UserName { get; set; } = null!;
        public string? Province { get; set; }
        public string? Municipality { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public IFormFile? Certificate { get; set; }
        public string? CertificatePassword { get; set; }
    }
}
