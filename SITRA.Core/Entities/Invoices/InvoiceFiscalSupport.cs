using SITRA.Core.Interfaces;
using SITRA.Core.Entities.AdministrativeTools;

namespace SITRA.Core.Entities.Invoices
{
    public class InvoiceFiscalSupport : ITenant, IAuditableEntity
    {
        public Guid InvoiceId { get; set; }
        public virtual Invoice Invoice { get; set; } = null!;
        public Guid FiscalSupportId { get; set; }
        public virtual FiscalSupport FiscalSupport { get; set; } = null!;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public Guid TenantId { get; set; }
    }
}
