using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SITRA.Core.Entities.Appointments;

namespace SITRA.Infrastructure.Persistence.Appointments
{
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            // Primary Key
            builder.HasKey(a => a.Id);

            // Properties
            builder.Property(a => a.Description)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(a => a.ScheduledAt)
                   .IsRequired();

            // Relationship: Appointment -> Contact (Many-to-One)
            builder.HasOne(a => a.Contact)
                   .WithMany(c => c.Appointments) // necesitas esta navegación en Contact
                   .HasForeignKey(a => a.ContactId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
