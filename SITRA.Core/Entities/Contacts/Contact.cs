using SITRA.Core.Interfaces;
using SITRA.Core.Entities.Appointments;
using SITRA.Core.Entities.Invoices;
using SITRA.Core.Enums;

namespace SITRA.Core.Entities.Contacts
{
    public sealed class Contact : ITenant
    {
        public long Id { get; set; }
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
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public Guid TenantId { get; set; }
        public ICollection<Invoice> BillerInvoices { get; set; } = new HashSet<Invoice>();
        public ICollection<Invoice> BuyerInvoices { get; set; } = new HashSet<Invoice>();
        public ICollection<Appointment> Appointments { get; set; } = new HashSet<Appointment>();
    }
}
