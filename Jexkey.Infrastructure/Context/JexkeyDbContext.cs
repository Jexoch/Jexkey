using Jexkey.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Jexkey.Infrastructure.Context {
    public class JexkeyDbContext : DbContext {
        public JexkeyDbContext(DbContextOptions<JexkeyDbContext> options) : base(options) {
        }

        public DbSet<Item> Items { get; set; }
        public DbSet<Vault> Vaults { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(JexkeyDbContext).Assembly);
        }
    }
}
