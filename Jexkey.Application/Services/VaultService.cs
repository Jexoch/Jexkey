using Jexkey.Application.DTOs.Vault;
using Jexkey.Application.Interfaces;
using Jexkey.Domain.Entities;
using Jexkey.Domain.Interfaces;

namespace Jexkey.Application.Services {
    public class VaultService : IVaultService {
        private readonly IVaultRepository _vaultRepository;

        public VaultService(IVaultRepository vaultRepository) {
            _vaultRepository = vaultRepository;
        }

        public async Task<VaultResponseDto> CreateVaultAsync(Guid userId, CreateVaultDto dto) {
            ArgumentNullException.ThrowIfNull(dto);

            if (dto.IsPasswordProtected && (dto.VaultPasswordHash == null || dto.VaultPasswordSalt == null)) {
                throw new ArgumentException("Password protected vaults require both password hash and salt.");
            }

            var vault = new Vault {
                UserId = userId,
                EncName = dto.EncName,
                EncVaultKey = dto.EncVaultKey,
                IsPasswordProtected = dto.IsPasswordProtected,
                VaultPasswordHash = dto.VaultPasswordHash != null ? (byte[])dto.VaultPasswordHash.Clone() : null,
                VaultPasswordSalt = dto.VaultPasswordSalt != null ? (byte[])dto.VaultPasswordSalt.Clone() : null
            };

            await _vaultRepository.InsertAsync(vault);

            return MapToResponseDto(vault);
        }

        public async Task<bool> DeleteVaultAsync(Guid vaultId, Guid userId) {
            var vault = await _vaultRepository.GetByIdAsync(vaultId);

            if (vault == null || vault.UserId != userId) {
                return false;
            }

            try {
                await _vaultRepository.DeleteAsync(vault);
                return true;
            } catch (Exception ex) {
                Console.WriteLine($"Error deleting vault {vaultId}: {ex.Message}");
                return false;
            }
        }

        public async Task<List<VaultResponseDto>> GetUserVaultsAsync(Guid userId) {
            var vaults = await _vaultRepository.GetVaultsByUserIdAsync(userId);
            return vaults.Select(MapToResponseDto).ToList();
        }

        public async Task<VaultResponseDto?> GetVaultByIdAsync(Guid vaultId, Guid userId) {
            var vault = await _vaultRepository.GetByIdAsync(vaultId);

            if (vault == null || vault.UserId != userId) {
                return null;
            }

            return MapToResponseDto(vault);
        }

        private static VaultResponseDto MapToResponseDto(Vault vault) {
            return new VaultResponseDto {
                Id = vault.Id,
                EncName = vault.EncName,
                EncVaultKey = vault.EncVaultKey,
                IsPasswordProtected = vault.IsPasswordProtected,
                VaultPasswordSalt = vault.VaultPasswordSalt,
                CreatedAt = vault.CreatedAt
            };
        }
    }
}
