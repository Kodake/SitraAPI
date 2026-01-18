using SITRA.Core.Interfaces;
using SITRA.Core.Enums;

namespace SITRA.Core.Entities.Invoices
{
    public class Payment : ITenant, IAuditableEntity
    {
        public long Id { get; set; }
        public long SpotPaymentId { get; set; }
        public SpotPayment SpotPayment { get; set; } = null!;
        public PaymentMethod PaymentMethod { get; set; }
        public double Amount { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public Guid TenantId { get; set; }
    }
}
