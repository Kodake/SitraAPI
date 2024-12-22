using BackEnd.Entities.Contacts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackEnd.Context.Contacts
{
    public class ContactConfiguration : IEntityTypeConfiguration<Contact>
    {
        public void Configure(EntityTypeBuilder<Contact> builder)
        {
            builder.Property(p => p.ContactType).HasConversion<int>().IsRequired(false);
            builder.Property(p => p.IdentificationType).HasConversion<int>().IsRequired(false);
            builder.Property(p => p.Province).HasMaxLength(2).IsRequired(false);
            builder.Property(p => p.Identification).HasMaxLength(15).IsRequired(false);
            builder.Property(p => p.Name).HasMaxLength(50);
            builder.Property(p => p.Address).HasMaxLength(100);
        }
    }
}