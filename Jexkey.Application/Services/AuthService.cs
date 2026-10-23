using Jexkey.Application.DTOs.Auth;
using Jexkey.Application.Interfaces;
using Jexkey.Domain.Entities;
using Jexkey.Domain.Interfaces;
using System.Security.Cryptography;

namespace Jexkey.Application.Services {
    public class AuthService : IAuthService {
        private const int ServerPbkdf2Iterations = 100_000;
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;

        public AuthService(IUserRepository userRepository, ITokenService tokenService) {
            _tokenService = tokenService;
            _userRepository = userRepository;
        }

        public async Task<byte[]?> GetMasterSaltAsync(string identity) {
            var user = await _userRepository.GetUserByEmailOrUsernameAsync(identity);
            if (user == null) {
                return RandomNumberGenerator.GetBytes(16);
            }
            return user.MasterSalt;
        }

        public async Task<string?> LoginAsync(LoginDto dto) {
            try {
                var user = await _userRepository.GetUserByEmailOrUsernameAsync(dto.Identity);
                if (user == null) {
                    // Timing mitigation: perform dummy PBKDF2 calculation so response time is equalized
                    var dummySalt = new byte[16];
                    _ = Rfc2898DeriveBytes.Pbkdf2(dto.MasterHash, dummySalt, ServerPbkdf2Iterations, HashAlgorithmName.SHA256, 32);
                    return null;
                }

                var computedHash = Rfc2898DeriveBytes.Pbkdf2(
                    dto.MasterHash, user.AuthSalt, ServerPbkdf2Iterations, HashAlgorithmName.SHA256, 32);

                try {
                    if (CryptographicOperations.FixedTimeEquals(computedHash, user.MasterHash)) {
                        return _tokenService.GenerateJwtToken(user);
                    }

                    return null;
                } finally {
                    Array.Clear(computedHash);
                }
            } finally {
                Array.Clear(dto.MasterHash);
            }
        }

        public async Task<bool> RegisterAsync(RegisterDto dto) {
            if (await _userRepository.ExistsByEmailOrUsernameAsync(dto.UserEmail) ||
                await _userRepository.ExistsByEmailOrUsernameAsync(dto.UserName)) {
                return false;
            }

            var authSalt = RandomNumberGenerator.GetBytes(16);
            var serverHashed = Rfc2898DeriveBytes.Pbkdf2(
                dto.MasterHash, authSalt, ServerPbkdf2Iterations, HashAlgorithmName.SHA256, 32);

            var user = new User {
                UserName = dto.UserName,
                UserEmail = dto.UserEmail,
                MasterHash = serverHashed,
                MasterSalt = (byte[])dto.MasterSalt.Clone(),
                AuthSalt = authSalt
            };

            try {
                await _userRepository.InsertAsync(user);
                return true;
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
            } finally {
                Array.Clear(dto.MasterHash);
                Array.Clear(dto.MasterSalt);
            }

            return false;
        }
    }
}
