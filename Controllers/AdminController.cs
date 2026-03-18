using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApplication1.Data;
using WebApplication1.DTOs;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly TwoFactorService _twoFactorService;

        public AdminController(AppDbContext context, TwoFactorService twoFactorService)
        {
            _context = context;
            _twoFactorService = twoFactorService;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users
                .Include(u => u.Role)
                .Select(u => new
                {
                    u.Id,
                    u.Username,
                    u.Email,
                    u.IsActive,
                    u.FailedLoginAttempts,
                    u.LockoutEnd,
                    Role = u.Role.Name
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpPut("users/{id}/toggle-active")]
        public async Task<IActionResult> ToggleUserActive(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.IsActive = !user.IsActive;
            await _context.SaveChangesAsync();

            return Ok(new { message = $"User {(user.IsActive ? "activated" : "deactivated")}" });
        }

        [HttpPost("2fa/setup")]
        public async Task<IActionResult> Setup2FA()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            var secret = _twoFactorService.GenerateSecret();
            user.TwoFactorSecret = secret;
            await _context.SaveChangesAsync();

            var qrUrl = _twoFactorService.GenerateQrCodeUrl(user.Email, secret);
            return Ok(new { secret, qrUrl });
        }

        [HttpPost("2fa/enable")]
        public async Task<IActionResult> Enable2FA(Enable2FARequest request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            if (string.IsNullOrEmpty(user.TwoFactorSecret))
                return BadRequest("Please setup 2FA first");

            if (!_twoFactorService.VerifyCode(user.TwoFactorSecret, request.Code))
                return BadRequest("Invalid code");

            user.TwoFactorEnabled = true;
            await _context.SaveChangesAsync();

            return Ok(new { message = "2FA enabled successfully" });
        }
    }
}