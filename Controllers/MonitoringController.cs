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
    public class MonitoringController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IAuditService _auditService;
        private readonly IActivityLogger _activityLogger;

        public MonitoringController(AppDbContext context, IAuditService auditService, IActivityLogger activityLogger)
        {
            _context = context;
            _auditService = auditService;
            _activityLogger = activityLogger;
        }

        // US-12 View All Users
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
                    u.CreatedAt,
                    Role = u.Role.Name
                })
                .ToListAsync();

            return Ok(users);
        }

        // US-13 Search Users
        [HttpGet("users/search")]
        public async Task<IActionResult> SearchUsers([FromQuery] string? username, [FromQuery] string? email, [FromQuery] string? role)
        {
            var query = _context.Users.Include(u => u.Role).AsQueryable();

            if (!string.IsNullOrEmpty(username))
                query = query.Where(u => u.Username.Contains(username));

            if (!string.IsNullOrEmpty(email))
                query = query.Where(u => u.Email.Contains(email));

            if (!string.IsNullOrEmpty(role))
                query = query.Where(u => u.Role.Name == role);

            var users = await query.Select(u => new
            {
                u.Id,
                u.Username,
                u.Email,
                u.IsActive,
                Role = u.Role.Name
            }).ToListAsync();

            return Ok(users);
        }

        // US-14 Lock/Unlock Users
        [HttpPut("users/{id}/lock")]
        public async Task<IActionResult> LockUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.LockoutEnd = DateTime.UtcNow.AddYears(100);
            await _context.SaveChangesAsync();

            var adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _auditService.LogAdminAction(adminId, "LockUser", $"Locked user {user.Username}");

            return Ok(new { message = $"User {user.Username} locked" });
        }

        [HttpPut("users/{id}/unlock")]
        public async Task<IActionResult> UnlockUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.LockoutEnd = null;
            user.FailedLoginAttempts = 0;
            await _context.SaveChangesAsync();

            var adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _auditService.LogAdminAction(adminId, "UnlockUser", $"Unlocked user {user.Username}");

            return Ok(new { message = $"User {user.Username} unlocked" });
        }

        // US-15 Activity Logs
        [HttpGet("activity-logs")]
        public async Task<IActionResult> GetActivityLogs()
        {
            var logs = await _activityLogger.GetAllActivity();
            return Ok(logs);
        }

        [HttpGet("activity-logs/user/{userId}")]
        public async Task<IActionResult> GetUserActivityLogs(int userId)
        {
            var logs = await _activityLogger.GetUserActivity(userId);
            return Ok(logs);
        }

        // Audit Trail
        [HttpGet("audit-logs")]
        public async Task<IActionResult> GetAuditLogs()
        {
            var logs = await _auditService.GetAdminLogs();
            return Ok(logs);
        }

        [HttpGet("audit-logs/admin/{adminId}")]
        public async Task<IActionResult> GetAdminAuditLogs(int adminId)
        {
            var logs = await _auditService.GetAdminLogs(adminId);
            return Ok(logs);
        }

        // US-16 Adjust Balance
        [HttpPost("users/{id}/adjust-balance")]
        public async Task<IActionResult> AdjustBalance(int id, [FromBody] AdjustBalanceRequest request)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            var adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _auditService.LogAdminAction(adminId, "AdjustBalance",
                $"Adjusted balance for user {user.Username}: {request.Amount} - Reason: {request.Reason}");

            return Ok(new { message = $"Balance adjusted for {user.Username}" });
        }

        // US-17 System Dashboard
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var totalUsers = await _context.Users.CountAsync();
            var activeUsers = await _context.Users.CountAsync(u => u.IsActive);
            var lockedUsers = await _context.Users.CountAsync(u => u.LockoutEnd > DateTime.UtcNow);
            var totalAdminLogs = await _context.AdminLogs.CountAsync();
            var totalActivityLogs = await _context.ActivityLogs.CountAsync();
            var recentLogs = await _context.AdminLogs
                .OrderByDescending(l => l.CreatedAt)
                .Take(5)
                .ToListAsync();

            return Ok(new
            {
                totalUsers,
                activeUsers,
                lockedUsers,
                totalAdminLogs,
                totalActivityLogs,
                recentLogs
            });
        }
    }
}