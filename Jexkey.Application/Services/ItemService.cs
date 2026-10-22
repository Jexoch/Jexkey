using Jexkey.Application.DTOs.Item;
using Jexkey.Application.Interfaces;
using Jexkey.Domain.Entities;
using Jexkey.Domain.Interfaces;

namespace Jexkey.Application.Services {
    public class ItemService : IItemService {
        private readonly IVaultRepository _vaultRepository;
        private readonly IItemRepository _itemRepository;

        public ItemService(IVaultRepository vaultRepository, IItemRepository itemRepository) {
            _vaultRepository = vaultRepository;
            _itemRepository = itemRepository;
        }

        public async Task<ItemResponseDto?> CreateItemAsync(Guid userId, CreateItemDto dto) {
            ArgumentNullException.ThrowIfNull(dto);

            if (!await IsVaultOwnerAsync(dto.VaultId, userId)) {
                return null;
            }

            var item = new Item {
                VaultId = dto.VaultId,
                ItemType = dto.ItemType,
                EncTitle = dto.EncTitle,
                EncPassword = dto.EncPassword,
                EncUsername = dto.EncUsername,
                EncUrl = dto.EncUrl,
                EncNote = dto.EncNote
            };

            await _itemRepository.InsertAsync(item);
            return MapToResponseDto(item);
        }

        public async Task<bool> DeleteItemAsync(Guid itemId, Guid userId) {
            var item = await _itemRepository.GetByIdAsync(itemId);

            if (item == null || !await IsVaultOwnerAsync(item.VaultId, userId)) {
                return false;
            }

            try {
                await _itemRepository.DeleteAsync(item);
                return true;
            } catch (Exception ex) {
                Console.WriteLine($"Error deleting item {itemId}: {ex.Message}");
                return false;
            }
        }

        public async Task<ItemResponseDto?> GetItemByIdAsync(Guid itemId, Guid userId) {
            var item = await _itemRepository.GetByIdAsync(itemId);

            if (item == null || !await IsVaultOwnerAsync(item.VaultId, userId)) {
                return null;
            }

            return MapToResponseDto(item);
        }

        public async Task<List<ItemResponseDto>> GetVaultItemsAsync(Guid vaultId, Guid userId) {
            if (!await IsVaultOwnerAsync(vaultId, userId)) {
                return new List<ItemResponseDto>();
            }

            var items = await _itemRepository.GetItemsByVaultIdAsync(vaultId);
            return items.Select(MapToResponseDto).ToList();
        }

        public async Task<ItemResponseDto?> UpdateItemAsync(Guid itemId, Guid userId, UpdateItemDto dto) {
            ArgumentNullException.ThrowIfNull(dto);

            var item = await _itemRepository.GetByIdAsync(itemId);

            if (item == null || !await IsVaultOwnerAsync(item.VaultId, userId)) {
                return null;
            }

            item.ItemType = dto.ItemType;
            item.EncTitle = dto.EncTitle;
            item.EncPassword = dto.EncPassword;
            item.EncUrl = dto.EncUrl;
            item.EncNote = dto.EncNote;
            item.EncUsername = dto.EncUsername;
            item.UpdatedAt = DateTimeOffset.UtcNow;

            await _itemRepository.UpdateAsync(item);

            return MapToResponseDto(item);
        }

        private async Task<bool> IsVaultOwnerAsync(Guid vaultId, Guid userId) {
            var vault = await _vaultRepository.GetByIdAsync(vaultId);
            return vault != null && vault.UserId == userId;
        }

        private static ItemResponseDto MapToResponseDto(Item item) {
            return new ItemResponseDto {
                Id = item.Id,
                VaultId = item.VaultId,
                ItemType = item.ItemType,
                EncTitle = item.EncTitle,
                EncPassword = item.EncPassword,
                EncNote = item.EncNote,
                EncUrl = item.EncUrl,
                EncUsername = item.EncUsername,
                UpdatedAt = item.UpdatedAt
            };
        }
    }
}
