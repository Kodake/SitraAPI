using BackEnd.Enums;

namespace BackEnd.DTOs.Contacts
{
    public class CreateContactDTO
    {
        public ContactType? ContactType { get; set; }
        public IdentificationType? IdentificationType { get; set; }
        public string? Identification { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Province { get; set; }
        public string Address { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Phone2 { get; set; }
        public string? Email { get; set; }
    }
}
