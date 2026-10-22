namespace Jexkey.Application.DTOs.Vault {
    public class VaultResponseDto {
        public Guid Id { get; set; }
        public required string EncName { get; set; }
        public required string EncVaultKey { get; set; }
        public bool IsPasswordProtected { get; set; }
        public byte[]? VaultPasswordSalt { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
