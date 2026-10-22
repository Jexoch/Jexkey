using Jexkey.Domain.Enums;

namespace Jexkey.Domain.Entities {
    public class Item {
        public Guid Id { get; private set; }
        public Guid VaultId { get; set; }
        public ItemType ItemType { get; set; }

        public required string EncTitle { get; set; }
        public string? EncUsername { get; set; }
        public string? EncPassword { get; set; }
        public string? EncNote { get; set; }
        public string? EncUrl { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }

        public virtual Vault Vault { get; set; }
    }
}
