using SITRA.Core.DTOs.Warehouses.Warehouses;
using SITRA.Core.Enums;

namespace SITRA.Core.DTOs.AdministrativeTools.FiscalSupports
{
    public class FiscalSupportDTO
    {
        public Guid Id { get; set; }
        public DocumentType DocumentType { get; set; }
        public EcfType FiscalSupportType { get; set; }
        public string Name { get; set; } = string.Empty;
        public long InitialSequence { get; set; }
        public long FinalSequence { get; set; }
        public string FiscalSupportNumber { get; set; } = string.Empty;
        public bool IsElectronic { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
