using Jexkey.Domain.Enums;

namespace Jexkey.Domain.Entities {
    public class User {
        public Guid Id { get; private set; }
        public UserRole UserRole { get; set; }
        public string UserEmail { get; set; }
        public string UserName { get; set; }

        public byte[] MasterHash { get; set; }
        public byte[] MasterSalt { get; set; }
        public byte[] AuthSalt { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public virtual ICollection<Vault> Vaults { get; set; }
    }
}
