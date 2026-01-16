using SITRA.Core.Entities.Appointments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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
