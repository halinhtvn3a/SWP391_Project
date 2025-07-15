using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourtCaller.Persistence.Configurations
{
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.HasKey(e => e.BookingId);
            builder.Property(e => e.BookingId).HasMaxLength(10);
            builder.Property(e => e.Id).IsRequired().HasMaxLength(450);
            builder.Property(e => e.BranchId).HasMaxLength(10);
            builder.Property(e => e.BookingDate).IsRequired();
            builder.Property(e => e.BookingType).IsRequired().HasMaxLength(50);
            builder.Property(e => e.NumberOfSlot).IsRequired();
            builder.Property(e => e.Status).IsRequired();
            builder.Property(e => e.TotalPrice).IsRequired().HasColumnType("decimal(18,2)");

            // Relationships
            builder
                .HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(e => e.Branch)
                .WithMany()
                .HasForeignKey(e => e.BranchId)
                .OnDelete(DeleteBehavior.SetNull);

            builder
                .HasMany(e => e.TimeSlots)
                .WithOne(e => e.Booking)
                .HasForeignKey(e => e.BookingId)
                .OnDelete(DeleteBehavior.SetNull);

            builder
                .HasMany(e => e.Payments)
                .WithOne(e => e.Booking)
                .HasForeignKey(e => e.BookingId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
