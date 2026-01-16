using SITRA.Core.DTOs.Warehouses.Warehouses;
using SITRA.Core.Enums;

namespace SITRA.Core.DTOs.AdministrativeTools.FiscalSupports
{
    public class FiscalSupportListDTO
    {
        public long Id { get; set; }
        public DocumentType DocumentType { get; set; }
        public FiscalSupportType FiscalSupportType { get; set; }
        public string Name { get; set; } = string.Empty;
        public long WarehouseId { get; set; }
        public WarehouseDTO Warehouse { get; set; } = null!;
        public bool IsActive { get; set; } = true;
    }
}
