using Jexkey.Domain.Entities;

namespace Jexkey.Domain.Interfaces {
    public interface IUserRepository : IGenericRepository<User> {
        Task<User?> GetUserByEmailOrUsernameAsync(string identity);
        Task<bool> ExistsByEmailOrUsernameAsync(string identity);
    }
}
