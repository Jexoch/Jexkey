using Jexkey.Application.DTOs.Item;
using Jexkey.Application.Interfaces;
using Jexkey.WebAPI.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jexkey.WebAPI.Controllers {
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase {
        private readonly IItemService _itemService;

        public ItemsController(IItemService itemService) {
            _itemService = itemService;
        }

        [HttpGet("vault/{vaultId}")]
        public async Task<IActionResult> GetAllItems(Guid vaultId) {
            var items = await _itemService.GetVaultItemsAsync(vaultId, User.GetUserId());
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetItemById(Guid id) {
            var item = await _itemService.GetItemByIdAsync(id, User.GetUserId());
            if (item == null) {
                return NotFound();
            }

            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> PostNewItem([FromBody] CreateItemDto dto) {
            var item = await _itemService.CreateItemAsync(User.GetUserId(), dto);
            if (item == null) {
                return BadRequest("Vault not found or does not belong to this user.");
            }

            return CreatedAtAction(nameof(GetItemById), new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutItem(Guid id, [FromBody] UpdateItemDto dto) {
            var item = await _itemService.UpdateItemAsync(id, User.GetUserId(), dto);
            if (item == null) {
                return NotFound();
            }

            return Ok(item);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(Guid id) {
            if (await _itemService.DeleteItemAsync(id, User.GetUserId())) {
                return NoContent();
            }

            return NotFound();
        }
    }
}
