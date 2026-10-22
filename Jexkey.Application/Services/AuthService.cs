using Jexkey.Application.DTOs.Auth;
using Jexkey.Application.Interfaces;
using Jexkey.Domain.Entities;
using Jexkey.Domain.Interfaces;
using System.Security.Cryptography;

namespace Jexkey.Application.Services {
    public class AuthService : IAuthService {
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
                    return null;
                }

                if (CryptographicOperations.FixedTimeEquals(dto.MasterHash, user.MasterHash)) {
                    return _tokenService.GenerateJwtToken(user);
                }

                return null;
            } finally {
                Array.Clear(dto.MasterHash);
            }
        }

        public async Task<bool> RegisterAsync(RegisterDto dto) {
            if (await _userRepository.ExistsByEmailOrUsernameAsync(dto.UserEmail) ||
                await _userRepository.ExistsByEmailOrUsernameAsync(dto.UserName)) {
                return false;
            }

            var user = new User {
                UserName = dto.UserName,
                UserEmail = dto.UserEmail,
                MasterHash = (byte[])dto.MasterHash.Clone(),
                MasterSalt = (byte[])dto.MasterSalt.Clone(),
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
