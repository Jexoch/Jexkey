using Jexkey.Domain.Entities;
using Jexkey.Domain.Interfaces;
using Jexkey.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Jexkey.Infrastructure.Repositories {
    public class VaultRepository : GenericRepository<Vault>, IVaultRepository {
        public VaultRepository(JexkeyDbContext dbContext) : base(dbContext) {
        }

        public async Task<List<Vault>> GetVaultsByUserIdAsync(Guid userId) {
            return await _context.Set<Vault>()
                .AsNoTracking()
                .Where(u => u.UserId == userId)
                .ToListAsync()
                .ConfigureAwait(false);
        }
    }
}
