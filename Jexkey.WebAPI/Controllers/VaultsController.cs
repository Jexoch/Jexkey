using Jexkey.Application.DTOs.Vault;
using Jexkey.Application.Interfaces;
using Jexkey.WebAPI.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jexkey.WebAPI.Controllers {
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class VaultsController : ControllerBase {
        private readonly IVaultService _vaultService;

        public VaultsController(IVaultService vaultService) {
            _vaultService = vaultService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllVaults() {
            var vaults = await _vaultService.GetUserVaultsAsync(User.GetUserId());
            return Ok(vaults);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVaultById(Guid id) {
            var vault = await _vaultService.GetVaultByIdAsync(id, User.GetUserId());
            if (vault == null) {
                return NotFound();
            }

            return Ok(vault);
        }

        [HttpPost]
        public async Task<IActionResult> CreateVault([FromBody] CreateVaultDto dto) {
            var vault = await _vaultService.CreateVaultAsync(User.GetUserId(), dto);
            return CreatedAtAction(nameof(GetVaultById), new { id = vault.Id }, vault);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVault(Guid id) {
            if (await _vaultService.DeleteVaultAsync(id, User.GetUserId())) {
                return NoContent();
            }

            return NotFound();
        }
    }
}
