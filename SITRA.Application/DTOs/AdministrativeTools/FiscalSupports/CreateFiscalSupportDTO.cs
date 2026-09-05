using SITRA.Core.Enums;

namespace SITRA.Core.DTOs.AdministrativeTools.FiscalSupports
{
    public class CreateFiscalSupportDTO
    {
        public DocumentType DocumentType { get; set; }
        public EcfType FiscalSupportType { get; set; }
        public string Name { get; set; } = string.Empty;
        public long InitialSequence { get; set; }
        public long FinalSequence { get; set; }
    }
}
