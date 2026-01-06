using BackEnd.Entities.Contacts;

namespace BackEnd.Entities.Appointments
{
    public sealed class Appointment
    {
        public long Id { get; set; }
        public string Description { get; set; } = null!;
        public long ContactId { get; set; }
        public Contact Contact { get; set; } = null!;
        public DateTimeOffset ScheduledAt { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public ICollection<AppointmentItem> AppointmentItems { get; set; }
            = new List<AppointmentItem>();
    }
}
