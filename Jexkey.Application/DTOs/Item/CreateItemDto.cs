using Jexkey.Domain.Enums;

namespace Jexkey.Application.DTOs.Item {
    public class CreateItemDto {
        public Guid VaultId { get; set; }
        public ItemType ItemType { get; set; }
        public required string EncTitle { get; set; }
        public string? EncPassword { get; set; }
        public string? EncUsername { get; set; }
        public string? EncUrl { get; set; }
        public string? EncNote { get; set; }
    }
}
