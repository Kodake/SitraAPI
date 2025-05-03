namespace BackEnd.DTOs
{
    public class TokenResponseDTO
    {
        public string Token { get; set; } = null!;
        public DateTime Expire { get; set; }
        public bool IsAuthenticated { get; set; }
    }
}
