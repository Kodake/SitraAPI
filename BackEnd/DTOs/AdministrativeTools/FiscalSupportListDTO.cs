using BackEnd.DTOs.Warehouses;
using BackEnd.Entities.Warehouses;
using BackEnd.Enums;

namespace BackEnd.DTOs.AdministrativeTools
{
    public class FiscalSupportListDTO
    {
        public int Id { get; set; }
        public DocumentType DocumentType { get; set; }
        public FiscalSupportType FiscalSupportType { get; set; }
        public string Name { get; set; } = string.Empty;
        public int WarehouseId { get; set; }
        public WarehouseDTO Warehouse { get; set; } = null!;
        public bool IsActive { get; set; } = true;
    }
}
