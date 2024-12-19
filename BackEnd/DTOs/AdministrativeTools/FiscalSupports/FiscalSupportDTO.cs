using BackEnd.DTOs.Warehouses.Warehouses;
using BackEnd.Enums;

namespace BackEnd.DTOs.AdministrativeTools.FiscalSupports
{
    public class FiscalSupportDTO
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
        public string FiscalSupportNumber => GenerateFiscalSupportNumber();
        private string GenerateFiscalSupportNumber()
        {
            string fiscalSupportName = Enum.GetName(typeof(FiscalSupportType), FiscalSupportType) ?? "Unknown";
            char symbolChar = Symbol.Length > 0 ? Symbol[0] : '?';
            int validSpaceNumber = (int)SpaceNumber;
            string repeatedSymbols = new string(symbolChar, validSpaceNumber);
            string sequence = ((int)InitialSequence).ToString().PadLeft(validSpaceNumber, symbolChar);
            string finalSymbols = repeatedSymbols[..^validSpaceNumber] + sequence;

            return $"{fiscalSupportName}{finalSymbols}";
        }
        public WarehouseDTO Warehouse { get; set; } = null!;
    }
}
