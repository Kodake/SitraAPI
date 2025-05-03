namespace BackEnd.DTOs.Auths
{
    public class CreateLoginUserDTO
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
