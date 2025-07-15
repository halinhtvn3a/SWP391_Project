using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourtCaller.Persistence.Configurations
{
    public class PriceConfiguration : IEntityTypeConfiguration<Price>
    {
        public void Configure(EntityTypeBuilder<Price> builder)
        {
            builder.HasKey(e => e.PriceId);
            builder.Property(e => e.PriceId).HasMaxLength(10);
            builder.Property(e => e.BranchId).IsRequired().HasMaxLength(10);
            builder.Property(e => e.Type).HasMaxLength(50);
            builder.Property(e => e.IsWeekend);
            builder.Property(e => e.SlotPrice).IsRequired().HasColumnType("decimal(18,2)");

            // Relationships
            builder
                .HasOne(e => e.Branch)
                .WithMany(e => e.Prices)
                .HasForeignKey(e => e.BranchId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
