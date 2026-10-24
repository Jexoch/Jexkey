using Jexkey.Application.DTOs.Auth;
using Jexkey.Application.Interfaces;
using Jexkey.Domain.Entities;
using Jexkey.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace Jexkey.Application.Services {
    public class AuthService : IAuthService {
        private const int ServerPbkdf2Iterations = 100_000;
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly byte[] _fakeSaltPepperBytes;

        public AuthService(IUserRepository userRepository, ITokenService tokenService, IConfiguration configuration) {
            _tokenService = tokenService;
            _userRepository = userRepository;

            var pepper = configuration["Security:FakeSaltPepper"]
                ?? throw new InvalidOperationException("Security:FakeSaltPepper configuration is missing!");
            _fakeSaltPepperBytes = Encoding.UTF8.GetBytes(pepper);
        }

        public async Task<byte[]?> GetMasterSaltAsync(string identity) {
            var user = await _userRepository.GetUserByEmailOrUsernameAsync(identity);
            if (user == null) {
                return GenerateDeterministicFakeSalt(identity);
            }
            return user.MasterSalt;
        }

        public async Task<string?> LoginAsync(LoginDto dto) {
            try {
                var user = await _userRepository.GetUserByEmailOrUsernameAsync(dto.Identity);
                if (user == null) {
                    // Timing mitigation: perform dummy PBKDF2 calculation so response time is equalized
                    var dummySalt = GenerateDeterministicFakeSalt(dto.Identity);
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

        private byte[] GenerateDeterministicFakeSalt(string identity) {
            byte[] identityBytes = Encoding.UTF8.GetBytes(identity);
            byte[] hash = HMACSHA256.HashData(_fakeSaltPepperBytes, identityBytes);
            return hash[..16];
        }
    }
}
