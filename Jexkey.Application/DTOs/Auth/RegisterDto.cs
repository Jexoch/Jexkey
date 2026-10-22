namespace Jexkey.Application.DTOs.Auth {
    public class RegisterDto {
        public required string UserEmail { get; set; }
        public required string UserName { get; set; }
        public required byte[] MasterHash { get; set; }
        public required byte[] MasterSalt { get; set; }
    }
}
