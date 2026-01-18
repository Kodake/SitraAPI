using SITRA.Core.Interfaces;
using SITRA.Core.Entities.Products;

namespace SITRA.Core.Entities.Appointments
{
    public sealed class AppointmentItem : ITenant, IAuditableEntity
    {
        public long AppointmentId { get; set; }
        public long ItemId { get; set; }
        public Appointment Appointment { get; set; } = null!;
        public Item Item { get; set; } = null!;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public Guid TenantId { get; set; }
    }
}
