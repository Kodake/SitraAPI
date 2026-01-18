using SITRA.Core.Enums;

namespace SITRA.Core.DTOs.CashBoxes
{
    public class CreatePettyCashMovementDTO
    {
        public long Id { get; set; }
        public long PettyCashId { get; set; }
        public long InvoiceId { get; set; }
        public double Amount { get; set; }
        public MovementType Type { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
