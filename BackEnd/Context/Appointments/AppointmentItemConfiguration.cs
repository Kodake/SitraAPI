using BackEnd.Entities.Appointments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackEnd.Context.Appointments
{
    public class AppointmentItemConfiguration
        : IEntityTypeConfiguration<AppointmentItem>
    {
        public void Configure(EntityTypeBuilder<AppointmentItem> builder)
        {
            // 🔑 CLAVE PRIMARIA COMPUESTA (ESTO SOLUCIONA EL ERROR)
            builder.HasKey(x => new { x.AppointmentId, x.ItemId });
        }
    }
}