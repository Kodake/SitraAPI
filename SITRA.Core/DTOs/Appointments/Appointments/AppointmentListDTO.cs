using SITRA.Core.Entities.Products;

namespace SITRA.Core.DTOs.Appointments.Appointments
{
    public class AppointmentListDTO
    {
        public Guid Id { get; set; }
        public string Description { get; set; } = null!;
        public string ContactName { get; set; } = null!;
        public string? ContactEmail { get; set; } = null!;
        public string? ContactPhone { get; set; } = null!;
        public DateTimeOffset ScheduledAt { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
