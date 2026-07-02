using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApplication1.Data;
using WebApplication1.DTOs;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JwtService _jwtService;
        private readonly PasswordService _passwordService;
        private readonly BankingService _bankingService;

        public AuthController(AppDbContext context, JwtService jwtService, PasswordService passwordService, BankingService bankingService)
        {
            _context = context;
            _jwtService = jwtService;
            _passwordService = passwordService;
            _bankingService = bankingService;
        }

        // US-01 Registration
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            if (!_passwordService.IsPasswordValid(request.Password))
                return BadRequest("Password must be 8+ chars, with uppercase, lowercase, number and special character");

            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                return BadRequest(new
                {
                    message = "Email already exists"
                });

            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
                return BadRequest("Username already exists");

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),

                PhoneNumber = request.PhoneNumber,
                NationalId = request.NationalId,
                Address = request.Address,

                DateOfBirth = request.DateOfBirth,
                AccountType = request.AccountType,

                RoleId = 2
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            await _passwordService.SavePasswordHistory(user);
            await _context.Entry(user).Reference(u => u.Role).LoadAsync();

            // Create a bank account for the new user automatically
            await _bankingService.CreateAccountAsync(user.Id);

            return Ok(new AuthResponse
            {
                Token = _jwtService.GenerateToken(user),
                Username = user.Username,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role.Name
            });
        }

        // US-02 Login
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
                return Unauthorized(new
                {
                    message = "Invalid email or password"
                });

            if (_passwordService.IsLockedOut(user))
                return Unauthorized($"Account locked. Try again after {user.LockoutEnd}");

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                await _passwordService.HandleFailedLogin(user);
                return Unauthorized("Invalid email or password");
            }

            if (!user.IsActive)
                return Unauthorized("Account is disabled");

            await _passwordService.ResetFailedAttempts(user);

            return Ok(new AuthResponse
            {
                Token = _jwtService.GenerateToken(user),
                Username = user.Username,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role.Name
            });
        }

        // US-09 Logout
        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            return Ok(new { message = "Logged out successfully" });
        }

        // US-07 Change Password
        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return NotFound();

            if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            {
                return BadRequest(new
                {
                    message = "Current password is incorrect"
                });
            }

            if (!_passwordService.IsPasswordValid(request.NewPassword))
            {
                return BadRequest(new
                {
                    message = "Password must contain uppercase, lowercase, number and special character"
                });
            }

            if (await _passwordService.IsPasswordUsedBefore(user, request.NewPassword))
            {
                return BadRequest(new
                {
                    message = "Cannot reuse last 3 passwords"
                });
            }

            await _passwordService.SavePasswordHistory(user);

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Password changed successfully"
            });
        }

        // US-10 Forgot Password
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
                return Ok(new { message = "If email exists, reset token will be sent" });

            user.ResetPasswordToken = _passwordService.GenerateResetToken();
            user.ResetPasswordTokenExpiry = DateTime.UtcNow.AddHours(1);
            await _context.SaveChangesAsync();

            // In production, send token via email. For now, return it directly for testing.
            return Ok(new { message = "Reset token generated", token = user.ResetPasswordToken });
        }

        // US-10 Reset Password
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            var decodedToken = Uri.UnescapeDataString(request.Token).Trim();

            if (user == null || string.IsNullOrEmpty(user.ResetPasswordToken)
    || user.ResetPasswordToken.Trim() != decodedToken)
            {
                return BadRequest(new
                {
                    message = "Invalid token"
                });
            }

            if (user.ResetPasswordTokenExpiry < DateTime.UtcNow)
                return BadRequest("Token expired");

            if (!_passwordService.IsPasswordValid(request.NewPassword))
                return BadRequest("Password must be 8+ chars, with uppercase, lowercase, number and special character");

            if (await _passwordService.IsPasswordUsedBefore(user, request.NewPassword))
                return BadRequest("Cannot reuse last 3 passwords");

            await _passwordService.SavePasswordHistory(user);

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.ResetPasswordToken = null;
            user.ResetPasswordTokenExpiry = null;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Password reset successfully" });
        }

        // Verify User Identity
        [HttpPost("verify-user")]
        public async Task<IActionResult> VerifyUser(VerifyUserDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u =>

                u.Username.ToLower().Trim() == request.FullName.ToLower().Trim()
                && u.PhoneNumber == request.PhoneNumber
                && u.Address != null
                && u.Address.ToLower().Trim() == request.Address.ToLower().Trim()
                && u.NationalId != null
                && u.NationalId.EndsWith(request.NationalId)
            );

            if (user == null)
            {
                return BadRequest(new
                {
                    message = "Information does not match our records"
                });
            }

            user.ResetPasswordToken = _passwordService.GenerateResetToken();
            Console.WriteLine("Generated Token: " + user.ResetPasswordToken);
            user.ResetPasswordTokenExpiry = DateTime.UtcNow.AddHours(1);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Identity verified successfully",
                token = user.ResetPasswordToken,
                email = user.Email
            });
        }

        [HttpPut("update-profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile(UpdateProfileDto request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return NotFound();

            user.Username = request.Username;
            user.Email = request.Email;
            user.PhoneNumber = request.PhoneNumber;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Profile updated successfully",
                username = user.Username,
                email = user.Email,
                phoneNumber = user.PhoneNumber
            });
        }
    }
}