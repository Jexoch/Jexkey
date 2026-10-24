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

            var pepper = configuration["Security:FakeSaltPepper"];

            if (string.IsNullOrWhiteSpace(pepper) || pepper.StartsWith("YOUR_")) {
                throw new InvalidOperationException("Security:FakeSaltPepper configuration is missing or invalid!");
            }

            _fakeSaltPepperBytes = Encoding.UTF8.GetBytes(pepper);
        }

        public async Task<byte[]?> GetMasterSaltAsync(string identity) {
            var normalizedIdentity = NormalizeIdentity(identity);
            var user = await _userRepository.GetUserByEmailOrUsernameAsync(normalizedIdentity);
            if (user == null) {
                return GenerateDeterministicFakeSalt(normalizedIdentity);
            }
            return user.MasterSalt;
        }

        public async Task<string?> LoginAsync(LoginDto dto) {
            try {
                if (dto.MasterHash == null || dto.MasterHash.Length != 32) {
                    throw new InvalidOperationException("MasterHash length is invalid. It must be exactly 32 bytes.");
                }

                var normalizedIdentity = NormalizeIdentity(dto.Identity);
                var user = await _userRepository.GetUserByEmailOrUsernameAsync(normalizedIdentity);
                if (user == null) {
                    var dummySalt = GenerateDeterministicFakeSalt(normalizedIdentity);
                    _ = Rfc2898DeriveBytes.Pbkdf2(dto.MasterHash, dummySalt, ServerPbkdf2Iterations, HashAlgorithmName.SHA256, 32);
                    return null;
                }

                var computedHash = Rfc2898DeriveBytes.Pbkdf2(dto.MasterHash, user.AuthSalt, ServerPbkdf2Iterations, HashAlgorithmName.SHA256, 32);

                try {
                    if (CryptographicOperations.FixedTimeEquals(computedHash, user.MasterHash)) {
                        return _tokenService.GenerateJwtToken(user);
                    }

                    return null;
                } finally {
                    Array.Clear(computedHash);
                }
            } finally {
                if (dto.MasterHash != null) {
                    Array.Clear(dto.MasterHash);
                }
            }
        }

        public async Task<bool> RegisterAsync(RegisterDto dto) {
            if (!CheckEmailFormat(dto.UserEmail)) {
                throw new InvalidOperationException("Invalid email format");
            }

            if (dto.MasterHash == null || dto.MasterHash.Length != 32 || dto.MasterSalt == null || dto.MasterSalt.Length != 16) {
                throw new InvalidOperationException("MasterHash must be 32 bytes and MasterSalt must be 16 bytes.");
            }

            var normalizedEmail = NormalizeIdentity(dto.UserEmail);
            var normalizedUserName = NormalizeIdentity(dto.UserName);

            if (await _userRepository.ExistsByEmailOrUsernameAsync(normalizedEmail) ||
                await _userRepository.ExistsByEmailOrUsernameAsync(normalizedUserName)) {
                return false;
            }

            var authSalt = RandomNumberGenerator.GetBytes(16);
            var serverHashed = Rfc2898DeriveBytes.Pbkdf2(
                dto.MasterHash, authSalt, ServerPbkdf2Iterations, HashAlgorithmName.SHA256, 32);

            var user = new User {
                UserName = normalizedUserName,
                UserEmail = normalizedEmail,
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
                if (dto.MasterHash != null) {
                    Array.Clear(dto.MasterHash);
                }
                if (dto.MasterSalt != null) {
                    Array.Clear(dto.MasterSalt);
                }
            }

            return false;
        }

        private byte[] GenerateDeterministicFakeSalt(string identity) {
            byte[] identityBytes = Encoding.UTF8.GetBytes(identity);
            byte[] hash = HMACSHA256.HashData(_fakeSaltPepperBytes, identityBytes);
            return hash[..16];
        }

        private static bool CheckEmailFormat(string? email) {
            if (string.IsNullOrWhiteSpace(email)) {
                return false;
            }
            if (!email.Contains('@') || !email.Contains('.')) {
                return false;
            }
            return true;
        }

        private static string NormalizeIdentity(string input) {
            return input.Trim().ToLowerInvariant();
        }
    }
}
