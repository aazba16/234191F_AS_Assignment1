using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace _234191F_AS_Assignment1.Model
{
    public class AuthDbContext : IdentityDbContext<Register> // Inherit from IdentityDbContext<IdentityUser>
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options)
            : base(options)
        {
        }

        public DbSet<AuditLog> AuditLogs { get; set; } // Your custom table
        public DbSet<PasswordHistory> PasswordHistories { get; set; }
    }
}
