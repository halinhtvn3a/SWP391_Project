using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CourtManagement.Models;

public partial class CourtBookingContext : DbContext
{
    public CourtBookingContext()
    {
    }

    public CourtBookingContext(DbContextOptions<CourtBookingContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Court> Courts { get; set; }

    public virtual DbSet<CourtSlot> CourtSlots { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserDetail> UserDetails { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=CourtDb;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__Booking__73951AED157D3D7C");

            entity.ToTable("Booking");

            entity.Property(e => e.BookingId)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.UserId)
                .HasMaxLength(5)
                .IsUnicode(false);

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Booking__UserId__31EC6D26");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__Comments__C3B4DFCA40228F9D");

            entity.Property(e => e.CommentId)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.CourtSlotId)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.DateReplied).HasColumnType("datetime");
            entity.Property(e => e.ReplyText)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UserId)
                .HasMaxLength(5)
                .IsUnicode(false);

            entity.HasOne(d => d.CourtSlot).WithMany(p => p.Comments)
                .HasForeignKey(d => d.CourtSlotId)
                .HasConstraintName("FK__Comments__CourtS__33D4B598");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Comments__UserId__32E0915F");
        });

        modelBuilder.Entity<Court>(entity =>
        {
            entity.HasKey(e => e.CourtId).HasName("PK__Court__C3A67C9A3210AC12");

            entity.ToTable("Court");

            entity.Property(e => e.CourtId)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Location)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Openday)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Picture)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<CourtSlot>(entity =>
        {
            entity.HasKey(e => e.CourtslotId).HasName("PK__CourtSlo__C632D55B123E7212");

            entity.ToTable("CourtSlot");

            entity.Property(e => e.CourtslotId)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.BookingId)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.CourtId)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.Price).HasColumnType("money");

            entity.HasOne(d => d.Booking).WithMany(p => p.CourtSlots)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__CourtSlot__Booki__35BCFE0A");

            entity.HasOne(d => d.Court).WithMany(p => p.CourtSlots)
                .HasForeignKey(d => d.CourtId)
                .HasConstraintName("FK__CourtSlot__Court__34C8D9D1");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payment__9B556A384382C024");

            entity.ToTable("Payment");

            entity.Property(e => e.PaymentId)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.BookingId)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.ExternalVnPayTransactionCode)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Method)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Paytime).HasColumnType("datetime");

            entity.HasOne(d => d.Booking).WithMany(p => p.Payments)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK__Payment__Booking__36B12243");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__8AFACE1A54A3586A");

            entity.ToTable("Role");

            entity.Property(e => e.RoleId)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CC4C21571AF2");

            entity.ToTable("User");

            entity.Property(e => e.UserId)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.Balance).HasColumnType("money");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Qrcode)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.RoleId)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__User__RoleId__37A5467C");
        });

        modelBuilder.Entity<UserDetail>(entity =>
        {
            entity.HasKey(e => e.UserDetailId).HasName("PK__UserDeta__564F56B24AA06B56");

            entity.ToTable("UserDetail");

            entity.Property(e => e.UserDetailId)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Fullname)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.UserId)
                .HasMaxLength(5)
                .IsUnicode(false);

            entity.HasOne(d => d.User).WithMany(p => p.UserDetails)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__UserDetai__UserI__38996AB5");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
