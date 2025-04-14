using BackEnd.Entities.Warehouses;
using BackEnd.Enums;

namespace BackEnd.Entities.AdministrativeTools
{
    public sealed class FiscalSupport
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
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
