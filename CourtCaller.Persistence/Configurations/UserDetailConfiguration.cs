using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourtCaller.Persistence.Configurations
{
	public class UserDetailConfiguration : IEntityTypeConfiguration<UserDetail>
	{
		public void Configure(EntityTypeBuilder<UserDetail> builder)
		{
			builder.HasKey(e => e.UserId);
			builder.Property(e => e.UserId).HasMaxLength(450);
			builder.Property(e => e.Balance).HasColumnType("decimal(18,2)");
			builder.Property(e => e.Point).HasColumnType("decimal(18,2)");
			builder.Property(e => e.FullName).HasMaxLength(50);
			builder.Property(e => e.Address).HasMaxLength(500);
			builder.Property(e => e.ProfilePicture).HasMaxLength(500);
			builder.Property(e => e.YearOfBirth);
			builder.Property(e => e.IsVip);

			// Relationships
			builder.HasOne(e => e.User)
				.WithOne()
				.HasForeignKey<UserDetail>(e => e.UserId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}