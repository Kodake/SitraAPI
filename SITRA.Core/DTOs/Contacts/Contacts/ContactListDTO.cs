using SITRA.Core.Enums;

namespace SITRA.Core.DTOs.Contacts.Contacts
{
    public class ContactListDTO
    {
        public Guid Id { get; set; }
        public ContactType? ContactType { get; set; }
        public string? Identification { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Province { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
