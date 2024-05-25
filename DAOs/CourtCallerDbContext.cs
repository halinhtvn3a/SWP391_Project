using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BusinessObjects;

public partial class CourtCallerDbContext : DbContext
{
    public CourtCallerDbContext()
    {
    }

    public CourtCallerDbContext(DbContextOptions<CourtCallerDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Branch> Branches { get; set; }

    public virtual DbSet<Court> Courts { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<TimeSlot> TimeSlots { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=CourtCallerDb;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__Booking__73951AED6654C2AF");

            entity.ToTable("Booking");

            entity.Property(e => e.BookingId)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.SlotId)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.UserId)
                .HasMaxLength(5)
                .IsUnicode(false);

            entity.HasOne(d => d.Slot).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.SlotId)
                .HasConstraintName("FK__Booking__SlotId__32E0915F");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Booking__UserId__31EC6D26");
        });

        modelBuilder.Entity<Branch>(entity =>
        {
            entity.HasKey(e => e.BranchId).HasName("PK__Branch__A1682FC5D7815CE8");

            entity.ToTable("Branch");

            entity.Property(e => e.BranchId)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.OpenDay)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Picture)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Court>(entity =>
        {
            entity.HasKey(e => e.CourtId).HasName("PK__Court__C3A67C9A211944F9");

            entity.ToTable("Court");

            entity.Property(e => e.CourtId)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.BranchId)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.CourtName).HasMaxLength(100);

            entity.HasOne(d => d.Branch).WithMany(p => p.Courts)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("FK__Court__BranchId__33D4B598");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payment__9B556A388982EF04");

            entity.ToTable("Payment");

            entity.Property(e => e.PaymentId)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.BookingId)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.PaymentAmount).HasColumnType("money");
            entity.Property(e => e.PaymentMessage)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.PaymentSignature)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Booking).WithMany(p => p.Payments)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK__Payment__Booking__34C8D9D1");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK__Review__74BC79CED3E43F16");

            entity.ToTable("Review");

            entity.Property(e => e.ReviewId)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.CourtId)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.ReviewDate).HasColumnType("datetime");
            entity.Property(e => e.ReviewText).HasMaxLength(255);
            entity.Property(e => e.UserId)
                .HasMaxLength(5)
                .IsUnicode(false);

            entity.HasOne(d => d.Court).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.CourtId)
                .HasConstraintName("FK__Review__CourtId__36B12243");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Review__UserId__35BCFE0A");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__8AFACE1A0D4587B3");

            entity.ToTable("Role");

            entity.Property(e => e.RoleId)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.RoleName)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TimeSlot>(entity =>
        {
            entity.HasKey(e => e.SlotId).HasName("PK__TimeSlot__0A124AAFA8D96B06");

            entity.ToTable("TimeSlot");

            entity.Property(e => e.SlotId)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.CourtId)
                .HasMaxLength(5)
                .IsUnicode(false);

            entity.HasOne(d => d.Court).WithMany(p => p.TimeSlots)
                .HasForeignKey(d => d.CourtId)
                .HasConstraintName("FK__TimeSlot__CourtI__37A5467C");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CC4CEBBF3D31");

            entity.ToTable("User");

            entity.Property(e => e.UserId)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.Balance).HasColumnType("money");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.FullName).HasMaxLength(50);
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.RoleId)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__User__RoleId__38996AB5");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
