using Jexkey.Application.DTOs.Auth;

namespace Jexkey.Application.Interfaces {
    public interface IAuthService {
        Task<byte[]?> GetMasterSaltAsync(string identity);
        Task<bool> RegisterAsync(RegisterDto dto);
        Task<string?> LoginAsync(LoginDto dto);
    }
}
