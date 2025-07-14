using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourtCaller.Persistence.Configurations
{
	public class NewsConfiguration : IEntityTypeConfiguration<News>
	{
		public void Configure(EntityTypeBuilder<News> builder)
		{
			builder.HasKey(e => e.NewId);
			builder.Property(e => e.NewId).HasMaxLength(10);
			builder.Property(e => e.Title).IsRequired().HasMaxLength(255);
			builder.Property(e => e.Content).IsRequired().HasColumnType("nvarchar(max)");
			builder.Property(e => e.PublicationDate).IsRequired();
			builder.Property(e => e.Image).HasColumnType("nvarchar(max)");
			builder.Property(e => e.Status).IsRequired().HasMaxLength(50);
			builder.Property(e => e.IsHomepageSlideshow).IsRequired();
		}
	}
}