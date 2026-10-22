using Jexkey.Domain.Entities;

namespace Jexkey.Domain.Interfaces {
    public interface IVaultRepository : IGenericRepository<Vault> {
        Task<List<Vault>> GetVaultsByUserIdAsync(Guid userId);
    }
}
