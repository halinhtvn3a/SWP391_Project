using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourtCaller.Persistence.Configurations
{
	public class ReviewConfiguration : IEntityTypeConfiguration<Review>
	{
		public void Configure(EntityTypeBuilder<Review> builder)
		{
			builder.HasKey(e => e.ReviewId);
			builder.Property(e => e.ReviewId).HasMaxLength(10);
			builder.Property(e => e.ReviewText).HasMaxLength(255);
			builder.Property(e => e.ReviewDate);
			builder.Property(e => e.Rating);
			builder.Property(e => e.Id).IsRequired().HasMaxLength(450);
			builder.Property(e => e.BranchId).IsRequired().HasMaxLength(10);

			// Relationships
			builder.HasOne(e => e.User)
				.WithMany()
				.HasForeignKey(e => e.Id)
				.OnDelete(DeleteBehavior.Cascade);

			builder.HasOne(e => e.Branch)
				.WithMany(e => e.Reviews)
				.HasForeignKey(e => e.BranchId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}