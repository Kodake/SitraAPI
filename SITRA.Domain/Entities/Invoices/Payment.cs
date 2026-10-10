using SITRA.Core.Enums;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Entities.Invoices
{
    public class Payment : ITenant, IAuditableEntity
    {
        public Guid Id { get; set; }
        public Guid SpotPaymentId { get; set; }
        public SpotPayment SpotPayment { get; set; } = null!;
        public PaymentMethod PaymentMethod { get; set; }
        public double Amount { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public Guid TenantId { get; set; }
    }
}
