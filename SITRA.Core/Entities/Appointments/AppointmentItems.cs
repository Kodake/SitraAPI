using SITRA.Core.Entities.Products;

namespace SITRA.Core.Entities.Appointments
{
    public sealed class AppointmentItem
    {
        public long AppointmentId { get; set; }
        public long ItemId { get; set; }
        public Appointment Appointment { get; set; } = null!;
        public Item Item { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
