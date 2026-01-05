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

            builder.HasOne(x => x.Appointment)
                .WithMany(a => a.AppointmentItems)
                .HasForeignKey(x => x.AppointmentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Item)
                .WithMany() // solo si NO navegas desde Item
                .HasForeignKey(x => x.ItemId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("AppointmentServiceItems");
        }
    }
}