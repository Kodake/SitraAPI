namespace BackEnd.DTOs.Auths
{
    public class CreateRegistrationUserDTO
    {
        public string RucNumber { get; set; } = null!;
        public string CompanyName { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
