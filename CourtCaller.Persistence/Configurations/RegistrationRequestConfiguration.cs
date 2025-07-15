using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourtCaller.Persistence.Configurations
{
    public class RegistrationRequestConfiguration : IEntityTypeConfiguration<RegistrationRequest>
    {
        public void Configure(EntityTypeBuilder<RegistrationRequest> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).ValueGeneratedOnAdd();
            builder.Property(e => e.Email).IsRequired().HasMaxLength(256);
            builder.Property(e => e.Password).IsRequired();
            builder.Property(e => e.FullName).IsRequired().HasMaxLength(200);
            builder.Property(e => e.Token).IsRequired();
            builder.Property(e => e.TokenExpiration).IsRequired();
        }
    }
}
