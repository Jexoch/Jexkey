using Jexkey.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jexkey.Infrastructure.Configurations {
    public class UserConfiguration : IEntityTypeConfiguration<User> {
        public void Configure(EntityTypeBuilder<User> builder) {
            builder.ToTable("Users");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.UserName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(u => u.UserEmail)
                .IsRequired()
                .HasMaxLength(256);

            builder.HasIndex(u => u.UserEmail)
                .IsUnique();

            builder.HasIndex(u => u.UserName)
                .IsUnique();

            builder.Property(u => u.MasterHash)
                .IsRequired();

            builder.Property(u => u.MasterSalt)
                .IsRequired();

            builder.Property(u => u.UserRole)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(u => u.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("NOW()");
        }
    }
}
