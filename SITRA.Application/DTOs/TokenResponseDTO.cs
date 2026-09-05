using System;

namespace SITRA.Core.DTOs
{
    public class TokenResponseDTO
    {
        public string Token { get; set; } = null!;
        public DateTimeOffset Expire { get; set; }
        public bool IsAuthenticated { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiration { get; set; }
    }
}
