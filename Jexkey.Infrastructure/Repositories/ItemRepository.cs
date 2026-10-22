using Jexkey.Domain.Entities;
using Jexkey.Domain.Interfaces;
using Jexkey.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Jexkey.Infrastructure.Repositories {
    public class ItemRepository : GenericRepository<Item>, IItemRepository {
        public ItemRepository(JexkeyDbContext dbContext) : base(dbContext) {
        }

        public async Task<List<Item>> GetItemsByVaultIdAsync(Guid vaultId) {
            return await _context.Set<Item>()
                .AsNoTracking()
                .Where(v => v.VaultId == vaultId)
                .ToListAsync()
                .ConfigureAwait(false);
        }
    }
}
