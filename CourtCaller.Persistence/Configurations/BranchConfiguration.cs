using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourtCaller.Persistence.Configurations
{
    public class BranchConfiguration : IEntityTypeConfiguration<Branch>
    {
        public void Configure(EntityTypeBuilder<Branch> builder)
        {
            builder.HasKey(e => e.BranchId);
            builder.Property(e => e.BranchId).HasMaxLength(10);
            builder.Property(e => e.BranchName).IsRequired().HasMaxLength(255);
            builder.Property(e => e.BranchAddress).IsRequired().HasMaxLength(255);
            builder.Property(e => e.BranchPhone).IsRequired().HasMaxLength(15);
            builder.Property(e => e.Description).HasMaxLength(255);
            builder.Property(e => e.BranchPicture).HasColumnType("nvarchar(max)");
            builder.Property(e => e.OpenTime).IsRequired();
            builder.Property(e => e.CloseTime).IsRequired();
            builder.Property(e => e.OpenDay).IsRequired().HasMaxLength(255);
            builder.Property(e => e.Status).IsRequired().HasMaxLength(50);

            // Relationships
            builder
                .HasMany(e => e.Courts)
                .WithOne(e => e.Branch)
                .HasForeignKey(e => e.BranchId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(e => e.Prices)
                .WithOne(e => e.Branch)
                .HasForeignKey(e => e.BranchId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(e => e.Reviews)
                .WithOne(e => e.Branch)
                .HasForeignKey(e => e.BranchId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
