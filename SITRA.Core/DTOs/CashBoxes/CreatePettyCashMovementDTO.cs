using SITRA.Core.Enums;

namespace SITRA.Core.DTOs.CashBoxes
{
    public class CreatePettyCashMovementDTO
    {
        public Guid Id { get; set; }
        public Guid PettyCashId { get; set; }
        public Guid InvoiceId { get; set; }
        public double Amount { get; set; }
        public MovementType Type { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
