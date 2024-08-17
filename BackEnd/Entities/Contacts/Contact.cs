using BackEnd.Enums;

namespace BackEnd.Entities.Contacts
{
    public sealed class Contact
    {
        public int Id { get; set; }
        public ContactType? ContactType { get; set; }
        public IdentificationType? IdentificationType { get; set; }
        public string? Identification { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Province { get; set; }
        public string Address { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
