using WebApplication1.Models;

namespace WebApplication1.Services
{
    public interface IActivityLogger
    {
        Task Log(int userId, string action, string entityType, string? entityId = null, string? oldValues = null, string? newValues = null);
        Task<IEnumerable<ActivityLog>> GetUserActivity(int userId);
        Task<IEnumerable<ActivityLog>> GetAllActivity();
    }

    public class ActivityLogger : IActivityLogger
    {
        private readonly Data.AppDbContext _context;

        public ActivityLogger(Data.AppDbContext context)
        {
            _context = context;
        }

        public async Task Log(int userId, string action, string entityType, string? entityId = null, string? oldValues = null, string? newValues = null)
        {
            _context.ActivityLogs.Add(new ActivityLog
            {
                UserId = userId,
                Action = action,
                EntityType = entityType,
                EntityId = entityId,
                OldValues = oldValues,
                NewValues = newValues
            });
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ActivityLog>> GetUserActivity(int userId)
        {
            return await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions
                .ToListAsync(_context.ActivityLogs.Where(a => a.UserId == userId));
        }

        public async Task<IEnumerable<ActivityLog>> GetAllActivity()
        {
            return await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions
                .ToListAsync(_context.ActivityLogs);
        }
    }
}