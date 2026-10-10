using SITRA.Core.DTOs.Contacts.Contacts;

namespace SITRA.Core.DTOs.Appointments.Appointments
{
    public class AppointmentDTO
    {
        public Guid Id { get; set; }
        public string Description { get; set; } = null!;
        public Guid ContactId { get; set; }
        public ContactDTO Contact { get; set; } = null!;
        public DateTimeOffset ScheduledAt { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public List<Guid> AppointmentItemIds { get; set; } = [];
        public List<ServiceDTO> AppointmentItems { get; set; } = [];
    }
}
