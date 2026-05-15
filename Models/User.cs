namespace WebApplication1.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        // Account Lock
        public int FailedLoginAttempts { get; set; } = 0;
        public DateTime? LockoutEnd { get; set; }

        // Password Reset
        public string? ResetPasswordToken { get; set; }
        public DateTime? ResetPasswordTokenExpiry { get; set; }

        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;

        // 2FA
        public bool TwoFactorEnabled { get; set; } = false;
        public string? TwoFactorSecret { get; set; }

        
        public string? PhoneNumber { get; set; }
        public string? NationalId { get; set; }
        public string? Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? AccountType { get; set; }
    }
}