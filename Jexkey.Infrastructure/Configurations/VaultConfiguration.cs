using Jexkey.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jexkey.Infrastructure.Configurations {
    public class VaultConfiguration : IEntityTypeConfiguration<Vault> {
        public void Configure(EntityTypeBuilder<Vault> builder) {
            builder.ToTable("Vaults");

            builder.HasKey(v => v.Id);

            builder.Property(v => v.EncName)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(v => v.EncVaultKey)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(v => v.IsPasswordProtected)
                .IsRequired();

            builder.Property(v => v.VaultPasswordHash);
            builder.Property(v => v.VaultPasswordSalt);

            builder.Property(v => v.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("NOW()");

            builder.HasOne(v => v.User)
                .WithMany(u => u.Vaults)
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
