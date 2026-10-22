using Jexkey.Domain.Interfaces;
using Jexkey.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Jexkey.Infrastructure.Repositories {
    public class GenericRepository<T> : IGenericRepository<T> where T : class {
        protected readonly JexkeyDbContext _context;

        public GenericRepository(JexkeyDbContext dbContext) {
            _context = dbContext;
        }

        public async Task DeleteAsync(T entity) {
            _context.Remove(entity);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<List<T>> GetAllAsync() {
            return await _context.Set<T>().AsNoTracking().ToListAsync().ConfigureAwait(false);
        }

        public async Task<T?> GetByIdAsync(Guid id) {
            return await _context.Set<T>().FindAsync(id).ConfigureAwait(false);
        }

        public async Task InsertAsync(T entity) {
            await _context.AddAsync(entity);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task UpdateAsync(T entity) {
            _context.Update(entity);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
