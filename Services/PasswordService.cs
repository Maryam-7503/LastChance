using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class PasswordService
    {
        private readonly AppDbContext _context;
        private const int MaxFailedAttempts = 5;
        private const int LockoutMinutes = 15;
        private const int PasswordHistoryCount = 3;

        public PasswordService(AppDbContext context)
        {
            _context = context;
        }

        // Password Policy
        public bool IsPasswordValid(string password)
        {
            if (password.Length < 8) return false;
            if (!password.Any(char.IsUpper)) return false;
            if (!password.Any(char.IsLower)) return false;
            if (!password.Any(char.IsDigit)) return false;
            if (!password.Any(c => !char.IsLetterOrDigit(c))) return false;
            return true;
        }

        // Account Lock
        public bool IsLockedOut(User user)
        {
            if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
                return true;
            return false;
        }

        public async Task HandleFailedLogin(User user)
        {
            user.FailedLoginAttempts++;
            if (user.FailedLoginAttempts >= MaxFailedAttempts)
            {
                user.LockoutEnd = DateTime.UtcNow.AddMinutes(LockoutMinutes);
                user.FailedLoginAttempts = 0;
            }
            await _context.SaveChangesAsync();
        }

        public async Task ResetFailedAttempts(User user)
        {
            user.FailedLoginAttempts = 0;
            user.LockoutEnd = null;
            await _context.SaveChangesAsync();
        }

        // Password History
        public async Task<bool> IsPasswordUsedBefore(User user, string newPassword)
        {
            var history = await _context.PasswordHistories
                .Where(p => p.UserId == user.Id)
                .OrderByDescending(p => p.CreatedAt)
                .Take(PasswordHistoryCount)
                .ToListAsync();

            return history.Any(p => BCrypt.Net.BCrypt.Verify(newPassword, p.PasswordHash));
        }

        public async Task SavePasswordHistory(User user)
        {
            _context.PasswordHistories.Add(new PasswordHistory
            {
                UserId = user.Id,
                PasswordHash = user.PasswordHash
            });
            await _context.SaveChangesAsync();
        }

        // Reset Token
        public string GenerateResetToken()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                .Replace("/", "_").Replace("+", "-").Substring(0, 20);
        }
    }
}