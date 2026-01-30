using SITRA.Core.Entities.Appointments;
using SITRA.Core.Entities.Invoices;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Entities.Contacts
{
    public sealed class Contact : ITenant, IAuditableEntity
    {
        public Guid Id { get; set; }
        public ContactType? ContactType { get; set; }
        public IdentificationType? IdentificationType { get; set; }
        public string? Identification { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Province { get; set; }
        public string Address { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Phone2 { get; set; }
        public string? Email { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public Guid TenantId { get; set; }
        public ICollection<InvoiceBiller> InvoiceBillers { get; set; } = new HashSet<InvoiceBiller>();
        public ICollection<InvoiceBuyer> InvoiceBuyers { get; set; } = new HashSet<InvoiceBuyer>();
        public ICollection<Appointment> Appointments { get; set; } = new HashSet<Appointment>();
    }
}
