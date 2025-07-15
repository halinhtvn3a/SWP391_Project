using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourtCaller.Persistence.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.HasKey(e => e.PaymentId);
            builder.Property(e => e.PaymentId).HasMaxLength(10);
            builder.Property(e => e.BookingId).IsRequired().HasMaxLength(10);
            builder.Property(e => e.PaymentAmount).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(e => e.PaymentDate).IsRequired();
            builder.Property(e => e.PaymentMessage).HasMaxLength(500);
            builder.Property(e => e.PaymentStatus).IsRequired().HasMaxLength(50);
            builder.Property(e => e.PaymentSignature).HasMaxLength(50);

            // Relationships
            builder
                .HasOne(e => e.Booking)
                .WithMany(e => e.Payments)
                .HasForeignKey(e => e.BookingId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
