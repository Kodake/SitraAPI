using BackEnd.Enums;

namespace BackEnd.DTOs.CashBoxes
{
    public class PettyCashMovementListDTO
    {
        public long Id { get; set; }
        public long InvoiceId { get; set; }
        public double Amount { get; set; }
        public MovementType Type { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
