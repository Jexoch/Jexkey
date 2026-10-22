namespace Jexkey.Application.DTOs.Vault {
    public class CreateVaultDto {
        public bool IsPasswordProtected { get; set; }
        public required string EncName { get; set; }
        public required string EncVaultKey { get; set; }
        public byte[]? VaultPasswordHash { get; set; }
        public byte[]? VaultPasswordSalt { get; set; }
    }
}
