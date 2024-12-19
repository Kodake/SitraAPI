using BackEnd.Enums;

namespace BackEnd.DTOs.Contacts.Contacts
{
    public class ContactListDTO
    {
        public long Id { get; set; }
        public ContactType? ContactType { get; set; }
        public string? Identification { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Province { get; set; }
        public bool IsActive { get; set; }
    }
}
