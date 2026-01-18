using SITRA.Core.Interfaces;
using SITRA.Core.Entities.Warehouses;
using SITRA.Core.Enums;

namespace SITRA.Core.Entities.AdministrativeTools
{
    public sealed class FiscalSupport : ITenant, IAuditableEntity
    {
        public long Id { get; set; }
        public DocumentType DocumentType { get; set; }
        public FiscalSupportType FiscalSupportType { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
        public long InitialSequence { get; set; }
        public long FinalSequence { get; set; }
        public long SpaceNumber { get; set; }
        public long WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; } = null!;
        public bool IsActive { get; set; } = true;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public Guid TenantId { get; set; }
    }
}
