namespace SITRA.Core.DTOs.Appointments.Appointments
{
    public class AppointmentItemDTO
    {
        public Guid ItemId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
