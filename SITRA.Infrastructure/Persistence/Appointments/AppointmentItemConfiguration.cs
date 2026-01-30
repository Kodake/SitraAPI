using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SITRA.Core.Entities.Appointments;

namespace SITRA.Infrastructure.Persistence.Appointments
{
    public class AppointmentItemConfiguration
        : IEntityTypeConfiguration<AppointmentItem>
    {
        public void Configure(EntityTypeBuilder<AppointmentItem> builder)
        {
            // ?? CLAVE PRIMARIA COMPUESTA (ESTO SOLUCIONA EL ERROR)
            builder.HasKey(x => new { x.AppointmentId, x.ItemId });
        }
    }
}
