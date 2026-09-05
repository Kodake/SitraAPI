namespace SITRA.Core.DTOs.Auths
{
    public class InviteSubUserDTO
    {
        public string Email { get; set; } = null!;
        public string RoleName { get; set; } = null!;
    }

    public class CompleteInvitationDTO
    {
        public string Token { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
