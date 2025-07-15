using BusinessObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace CourtCaller.Persistence
{
    public class CourtCallerDbContext : IdentityDbContext, IApplicationDbContext
    {
        public CourtCallerDbContext(DbContextOptions<CourtCallerDbContext> options)
            : base(options)
        {
        }

        public CourtCallerDbContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

                IConfigurationRoot configurationRoot = builder.Build();
                optionsBuilder.UseSqlServer(configurationRoot.GetConnectionString("CourtCallerDb"));
            }
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(cancellationToken);
        }

        public virtual DbSet<Review> Reviews { get; set; }
        public virtual DbSet<UserDetail> UserDetails { get; set; }
        public virtual DbSet<Branch> Branches { get; set; }
        public virtual DbSet<Court> Courts { get; set; }
        public virtual DbSet<TimeSlot> TimeSlots { get; set; }
        public virtual DbSet<Booking> Bookings { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<Price> Prices { get; set; }
        public virtual DbSet<News> News { get; set; }
        public virtual DbSet<RegistrationRequest> RegistrationRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed roles
            // modelBuilder.Entity<IdentityRole>().HasData(
            //     new IdentityRole
            //     {
            //         Id = "R001",
            //         Name = "Admin",
            //         NormalizedName = "ADMIN",
            //         ConcurrencyStamp = Guid.NewGuid().ToString()
            //     },
            //     new IdentityRole
            //     {
            //         Id = "R002",
            //         Name = "Staff",
            //         NormalizedName = "STAFF",
            //         ConcurrencyStamp = Guid.NewGuid().ToString()
            //     },
            //     new IdentityRole
            //     {
            //         Id = "R003",
            //         Name = "Customer",
            //         NormalizedName = "CUSTOMER",
            //         ConcurrencyStamp = Guid.NewGuid().ToString()
            //     }
            // );
            // Apply all entity configurations
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CourtCallerDbContext).Assembly);
        }
    }
}
