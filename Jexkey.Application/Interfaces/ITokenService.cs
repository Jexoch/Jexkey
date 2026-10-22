using Jexkey.Domain.Entities;

namespace Jexkey.Application.Interfaces {
    public interface ITokenService {
        string GenerateJwtToken(User user);
    }
}
