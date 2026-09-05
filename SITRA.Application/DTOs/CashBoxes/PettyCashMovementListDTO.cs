using SITRA.Core.Enums;

namespace SITRA.Core.DTOs.CashBoxes
{
    public class PettyCashMovementListDTO
    {
        public Guid Id { get; set; }
        public Guid InvoiceId { get; set; }
        public string? FiscalSupportNumber { get; set; } = string.Empty;
        public double Amount { get; set; }
        public MovementType Type { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
