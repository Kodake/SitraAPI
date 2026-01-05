using BackEnd.DTOs.Contacts.Contacts;

namespace BackEnd.DTOs.Appointments.Appointments
{
    public class AppointmentDTO
    {
        public long Id { get; set; }
        public string Description { get; set; } = null!;
        public long ContactId { get; set; }
        public ContactDTO Contact { get; set; } = null!;
        public DateTimeOffset ScheduledAt { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public List<long> AppointmentItemIds { get; set; } = [];
        public List<ServiceDTO> AppointmentItems { get; set; } = [];
    }
}
