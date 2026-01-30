using SITRA.Core.Entities.Contacts;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Entities.Appointments
{
    public sealed class Appointment : ITenant, IAuditableEntity
    {
        public Guid Id { get; set; }
        public string Description { get; set; } = null!;
        public Guid ContactId { get; set; }
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
