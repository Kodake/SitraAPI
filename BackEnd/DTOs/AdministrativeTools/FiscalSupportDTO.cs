using BackEnd.DTOs.Warehouses;
using BackEnd.Entities.Warehouses;
using BackEnd.Enums;

namespace BackEnd.DTOs.AdministrativeTools
{
    public class FiscalSupportDTO
    {
        public int Id { get; set; }
        public DocumentType DocumentType { get; set; }
        public FiscalSupportType FiscalSupportType { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
        public int InitialSequence { get; set; }
        public int FinalSequence { get; set; }
        public int SpaceNumber { get; set; }
        public int WarehouseId { get; set; }
        public WarehouseDTO Warehouse { get; set; } = null!;
    }
}
