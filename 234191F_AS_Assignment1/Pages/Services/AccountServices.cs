using Microsoft.EntityFrameworkCore;
using _234191F_AS_Assignment1.Model;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace _234191F_AS_Assignment1.Pages.Services
{
    public class AccountServices
    {
        private readonly AuthDbContext _context;
        private readonly IPasswordHasher<Register> _passwordHasher;

        public AccountServices(AuthDbContext context, IPasswordHasher<Register> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task<bool> CheckPasswordHistory(string userId, string newPassword)
        {
            var passwordHistory = await _context.PasswordHistories
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.DateChanged)
                .Take(2)  // Get last 2 passwords
                .ToListAsync();

            foreach (var history in passwordHistory)
            {
                // Compare the new password with previous ones using password hashing
                if (_passwordHasher.VerifyHashedPassword(new Register(), history.HashedPassword, newPassword) == PasswordVerificationResult.Success)
                {
                    return false; // Password is reused
                }
            }
            return true; // Password is not reused
        }

        // Add new password to history after successful password change
        public async Task AddPasswordToHistory(string userId, string newPassword)
        {
            var passwordHistory = new PasswordHistory
            {
                UserId = userId,
                HashedPassword = _passwordHasher.HashPassword(new Register(), newPassword),  // Hash the new password
                DateChanged = DateTime.UtcNow
            };

            _context.PasswordHistories.Add(passwordHistory);
            await _context.SaveChangesAsync();
        }


    }
}
