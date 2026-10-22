using Jexkey.Application.DTOs.Vault;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jexkey.Application.Interfaces {
    public interface IVaultService {
        Task<List<VaultResponseDto>> GetUserVaultsAsync(Guid userId);
        Task<VaultResponseDto?> GetVaultByIdAsync(Guid vaultId, Guid userId);
        Task<VaultResponseDto> CreateVaultAsync(Guid userId, CreateVaultDto dto);
        Task<bool> DeleteVaultAsync(Guid vaultId, Guid userId);
    }
}
