using System;

namespace SITRA.Core.Entities.AdministrativeTools
{
    public class AuditLog : IEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? UserId { get; set; }
        public string Type { get; set; } = null!; // "Create", "Update", "Delete"
        public string TableName { get; set; } = null!;
        public DateTime DateTime { get; set; }
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public string? AffectedColumns { get; set; }
        public string? PrimaryKey { get; set; }
    }
}
