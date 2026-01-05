namespace BackEnd.DTOs.Appointments.Appointments
{
    public class CreateAppointmentDTO
    {
        public string Description { get; set; } = null!;
        public long ContactId { get; set; }
        public DateTimeOffset ScheduledAt { get; set; }
        public List<long> AppointmentItemIds { get; set; } = [];
    }
}
