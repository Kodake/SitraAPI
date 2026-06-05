using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SITRA.Core.Entities.AdministrativeTools;
using System.Text.Json;

namespace SITRA.Infrastructure.Persistence.Interceptors
{
    public class AuditInterceptor : SaveChangesInterceptor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditInterceptor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            if (eventData.Context is not null)
            {
                var auditEntries = CreateAuditEntries(eventData.Context);
                if (auditEntries.Count > 0)
                {
                    eventData.Context.Set<AuditLog>().AddRange(auditEntries);
                }
            }
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            if (eventData.Context is not null)
            {
                var auditEntries = CreateAuditEntries(eventData.Context);
                if (auditEntries.Count > 0)
                {
                    eventData.Context.Set<AuditLog>().AddRange(auditEntries);
                }
            }
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private List<AuditLog> CreateAuditEntries(DbContext context)
        {
            // The JWT token stores the user id in the "id" claim typically, or NameIdentifier
            var userId = _httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(c => c.Type == "id" || c.Type == "nameid" || c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                         ?? _httpContextAccessor.HttpContext?.User?.Identity?.Name
                         ?? "System";

            context.ChangeTracker.DetectChanges();
            var auditEntries = new List<AuditLog>();

            foreach (var entry in context.ChangeTracker.Entries())
            {
                if (entry.Entity is AuditLog || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                    continue;

                var auditEntry = new AuditLog
                {
                    TableName = entry.Metadata.GetTableName() ?? entry.Metadata.Name,
                    UserId = userId,
                    DateTime = DateTime.UtcNow,
                    Type = entry.State.ToString()
                };

                var oldValues = new Dictionary<string, object?>();
                var newValues = new Dictionary<string, object?>();
                var affectedColumns = new List<string>();

                foreach (var property in entry.Properties)
                {
                    if (property.IsTemporary)
                        continue;

                    string propertyName = property.Metadata.Name;

                    if (property.Metadata.IsPrimaryKey())
                    {
                        auditEntry.PrimaryKey = property.CurrentValue?.ToString();
                    }

                    switch (entry.State)
                    {
                        case EntityState.Added:
                            newValues[propertyName] = property.CurrentValue;
                            break;
                        case EntityState.Deleted:
                            oldValues[propertyName] = property.OriginalValue;
                            break;
                        case EntityState.Modified:
                            if (property.IsModified)
                            {
                                affectedColumns.Add(propertyName);
                                oldValues[propertyName] = property.OriginalValue;
                                newValues[propertyName] = property.CurrentValue;
                            }
                            break;
                    }
                }

                auditEntry.OldValues = oldValues.Count == 0 ? null : JsonSerializer.Serialize(oldValues);
                auditEntry.NewValues = newValues.Count == 0 ? null : JsonSerializer.Serialize(newValues);
                auditEntry.AffectedColumns = affectedColumns.Count == 0 ? null : JsonSerializer.Serialize(affectedColumns);

                auditEntries.Add(auditEntry);
            }

            return auditEntries;
        }
    }
}
