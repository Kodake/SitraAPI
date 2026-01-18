using SITRA.Core.Interfaces;
using SITRA.Core.Entities.Contacts;

namespace SITRA.Core.Entities.Appointments
{
    public sealed class Appointment : ITenant, IAuditableEntity
    {
        public long Id { get; set; }
        public string Description { get; set; } = null!;
        public long ContactId { get; set; }
        public Contact Contact { get; set; } = null!;
        public DateTimeOffset ScheduledAt { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public ICollection<AppointmentItem> AppointmentItems { get; set; }
            = new List<AppointmentItem>();
        public Guid TenantId { get; set; }
    }
}
