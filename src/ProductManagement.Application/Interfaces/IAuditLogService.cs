namespace ProductManagement.Application.Interfaces
{
    public interface IAuditLogService
    {
        Task LogAsync(string action, string entityName, int entityId, string details, int performedBy);
    }
}
