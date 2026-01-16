using SITRA.Core.Entities.Finances;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SITRA.Infrastructure.Persistence.Finances
{
    public class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
    {
        public void Configure(EntityTypeBuilder<Expense> builder)
        {
            builder.Property(p => p.Description).HasMaxLength(50);
            builder.Property(p => p.Amount).HasColumnType("decimal(18,2)").IsRequired();
        }
    }
}
