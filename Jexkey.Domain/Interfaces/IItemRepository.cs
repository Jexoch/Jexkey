using Jexkey.Domain.Entities;

namespace Jexkey.Domain.Interfaces {
    public interface IItemRepository : IGenericRepository<Item> {
        Task<List<Item>> GetItemsByVaultIdAsync(Guid vaultId);
    }
}
