using SITRA.Core.Enums;

namespace SITRA.Core.DTOs.CashBoxes
{
    public class WithdrawalPettyCashMovementListDTO
    {
        public Guid Id { get; set; }
        public double Amount { get; set; }
        public MovementType Type { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
