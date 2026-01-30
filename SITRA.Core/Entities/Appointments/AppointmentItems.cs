using SITRA.Core.Entities.Products;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Entities.Appointments
{
    public sealed class AppointmentItem : ITenant, IAuditableEntity
    {
        public Guid AppointmentId { get; set; }
        public Guid ItemId { get; set; }
        public Appointment Appointment { get; set; } = null!;
        public Item Item { get; set; } = null!;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public Guid TenantId { get; set; }
    }
}
