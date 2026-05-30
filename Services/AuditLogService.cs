using ProductManagement.Data;
using ProductManagement.Interfaces;
using ProductManagement.Model;
using Serilog;

namespace ProductManagement.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly AppDbContext _context;
        private readonly Serilog.ILogger _logger;

        public AuditLogService(AppDbContext context)
        {
            _context = context;
            _logger = Log.ForContext<AuditLogService>();
        }

        public async Task LogAsync(string action, string entityName, int entityId, string details, int performedBy)
        {
            var auditLog = new AuditLog
            {
                Action = action,
                EntityName = entityName,
                EntityId = entityId,
                Details = details,
                PerformedBy = performedBy,
                Timestamp = DateTime.UtcNow
            };

            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();

            _logger.Information("Audit: {Action} on {EntityName} (Id: {EntityId}) by User {PerformedBy}. {Details}",
                action, entityName, entityId, performedBy, details);
        }
    }
}
