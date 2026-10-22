using Jexkey.Application.DTOs.Auth;
using Jexkey.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Jexkey.WebAPI.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService) {
            _authService = authService;
        }

        [HttpGet("salt/{identity}")]
        public async Task<IActionResult> GetMasterSalt(string identity) {
            var salt = await _authService.GetMasterSaltAsync(identity);
            return Ok(salt);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto) {
            if (await _authService.RegisterAsync(dto)) {
                return Ok("Registered Successfully");
            }
            return BadRequest("This user already exist");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto) {
            var token = await _authService.LoginAsync(dto);
            if (token == null) {
                return Unauthorized();
            }
            return Ok(token);
        }
    }
}
