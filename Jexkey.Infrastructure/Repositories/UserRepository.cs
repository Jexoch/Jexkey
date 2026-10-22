using Jexkey.Domain.Entities;
using Jexkey.Domain.Interfaces;
using Jexkey.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Jexkey.Infrastructure.Repositories {
    public class UserRepository : GenericRepository<User>, IUserRepository {
        public UserRepository(JexkeyDbContext dbContext) : base(dbContext) {
        }

        public async Task<User?> GetUserByEmailOrUsernameAsync(string identity) {
            if (identity.Contains("@")) {
                return await _context.Set<User>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.UserEmail == identity)
                    .ConfigureAwait(false);
            } else {
                return await _context.Set<User>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.UserName == identity)
                    .ConfigureAwait(false);
            }
        }

        public async Task<bool> ExistsByEmailOrUsernameAsync(string identiy) {
            if (identiy.Contains("@")) {
                return await _context.Set<User>()
                    .AnyAsync(e => e.UserEmail == identiy)
                    .ConfigureAwait(false);
            } else {
                return await _context.Set<User>()
                    .AnyAsync(u => u.UserName == identiy)
                    .ConfigureAwait(false);
            }
        }
    }
}
