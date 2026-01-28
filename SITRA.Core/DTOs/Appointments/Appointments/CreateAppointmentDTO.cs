namespace SITRA.Core.DTOs.Appointments.Appointments
{
    public class CreateAppointmentDTO
    {
        public string Description { get; set; } = null!;
        public Guid ContactId { get; set; }
        public DateTimeOffset ScheduledAt { get; set; }
        public List<Guid> AppointmentItemIds { get; set; } = [];
    }
}
