using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourtCaller.Persistence.Configurations
{
	public class TimeSlotConfiguration : IEntityTypeConfiguration<TimeSlot>
	{
		public void Configure(EntityTypeBuilder<TimeSlot> builder)
		{
			builder.HasKey(e => e.SlotId);
			builder.Property(e => e.SlotId).HasMaxLength(10);
			builder.Property(e => e.CourtId).IsRequired().HasMaxLength(10);
			builder.Property(e => e.BookingId).HasMaxLength(10);
			builder.Property(e => e.SlotDate).IsRequired();
			builder.Property(e => e.Price).IsRequired().HasColumnType("decimal(18,2)");
			builder.Property(e => e.SlotStartTime).IsRequired();
			builder.Property(e => e.SlotEndTime).IsRequired();
			builder.Property(e => e.Status).IsRequired().HasMaxLength(50);
			builder.Property(e => e.Created_at);

			// Relationships
			builder.HasOne(e => e.Court)
				.WithMany(e => e.TimeSlots)
				.HasForeignKey(e => e.CourtId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.HasOne(e => e.Booking)
				.WithMany(e => e.TimeSlots)
				.HasForeignKey(e => e.BookingId)
				.OnDelete(DeleteBehavior.SetNull);
		}
	}
}