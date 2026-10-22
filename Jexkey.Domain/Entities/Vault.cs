namespace Jexkey.Domain.Entities {
    public class Vault {
        public Guid Id { get; private set; }
        public Guid UserId { get; set; }
        public string EncName { get; set; }
        public string EncVaultKey { get; set; }

        public bool IsPasswordProtected { get; set; }
        public byte[]? VaultPasswordHash { get; set; }
        public byte[]? VaultPasswordSalt { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

        public virtual User User { get; set; }
        public ICollection<Item> Items { get; set; }
    }
}
