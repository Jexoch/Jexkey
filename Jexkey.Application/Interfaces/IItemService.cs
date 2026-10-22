using Jexkey.Application.DTOs.Item;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jexkey.Application.Interfaces {
    public interface IItemService {
        Task<List<ItemResponseDto>> GetVaultItemsAsync(Guid vaultId, Guid userId);
        Task<ItemResponseDto?> GetItemByIdAsync(Guid itemId, Guid userId);
        Task<ItemResponseDto?> CreateItemAsync(Guid userId, CreateItemDto dto);
        Task<ItemResponseDto?> UpdateItemAsync(Guid itemId, Guid userId, UpdateItemDto dto);
        Task<bool> DeleteItemAsync(Guid itemId, Guid userId);
    }
}
