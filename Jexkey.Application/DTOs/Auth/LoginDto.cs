namespace Jexkey.Application.DTOs.Auth {
    public class LoginDto {
        public required string Identity { get; set; }
        public required byte[] MasterHash { get; set; }
    }
}
