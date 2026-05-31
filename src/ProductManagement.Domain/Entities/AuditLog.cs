namespace ProductManagement.Domain.Entities
{
    public class AuditLog
    {
        public int Id { get; set; }
        public string Action { get; set; } = string.Empty;
        public string EntityName { get; set; } = string.Empty;
        public int EntityId { get; set; }
        public string Details { get; set; } = string.Empty;
        public int PerformedBy { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
