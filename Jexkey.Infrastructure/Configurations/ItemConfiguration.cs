using Jexkey.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jexkey.Infrastructure.Configurations {
    public class ItemConfiguration : IEntityTypeConfiguration<Item> {
        public void Configure(EntityTypeBuilder<Item> builder) {
            builder.ToTable("Items");

            builder.HasKey(i => i.Id);

            builder.Property(i => i.EncTitle)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(i => i.EncUsername)
                .HasMaxLength(1000);

            builder.Property(i => i.EncPassword)
                .HasMaxLength(1000);

            builder.Property(i => i.EncUrl)
                .HasMaxLength(2000);

            builder.Property(i => i.EncNote)
                .HasMaxLength(10000);

            builder.Property(i => i.ItemType)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(i => i.UpdatedAt)
                .IsRequired()
                .HasDefaultValueSql("NOW()");

            builder.HasOne(i => i.Vault)
                .WithMany(v => v.Items)
                .HasForeignKey(i => i.VaultId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
