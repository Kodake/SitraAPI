using SITRA.Core.Enums;

namespace SITRA.Core.DTOs.CashBoxes
{
    public class PettyCashMovementListDTO
    {
        public long Id { get; set; }
        public long InvoiceId { get; set; }
        public string? FiscalSupportNumber { get; set; } = string.Empty;
        public double Amount { get; set; }
        public MovementType Type { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
