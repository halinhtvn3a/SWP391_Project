using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourtCaller.Persistence.Configurations
{
	public class CourtConfiguration : IEntityTypeConfiguration<Court>
	{
		public void Configure(EntityTypeBuilder<Court> builder)
		{
			builder.HasKey(e => e.CourtId);
			builder.Property(e => e.CourtId).HasMaxLength(10);
			builder.Property(e => e.BranchId).IsRequired().HasMaxLength(10);
			builder.Property(e => e.CourtName).IsRequired().HasMaxLength(100);
			builder.Property(e => e.CourtPicture).HasMaxLength(450);
			builder.Property(e => e.Status).IsRequired().HasMaxLength(100);

			// Relationships
			builder.HasOne(e => e.Branch)
				.WithMany(e => e.Courts)
				.HasForeignKey(e => e.BranchId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.HasMany(e => e.TimeSlots)
				.WithOne(e => e.Court)
				.HasForeignKey(e => e.CourtId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}