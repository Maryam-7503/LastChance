using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public interface IAuditService
    {
        Task LogAdminAction(int adminId, string action, string details);
        Task<IEnumerable<AdminLog>> GetAdminLogs(int? adminId = null);
    }

    public class AuditService : IAuditService
    {
        private readonly Data.AppDbContext _context;

        public AuditService(Data.AppDbContext context)
        {
            _context = context;
        }

        public async Task LogAdminAction(int adminId, string action, string details)
        {
            _context.AdminLogs.Add(new AdminLog
            {
                AdminId = adminId,
                Action = action,
                Details = details
            });
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<AdminLog>> GetAdminLogs(int? adminId = null)
        {
            var query = _context.AdminLogs.AsQueryable();
            if (adminId.HasValue)
                query = query.Where(l => l.AdminId == adminId);
            return await query.ToListAsync();
        }
    }
}