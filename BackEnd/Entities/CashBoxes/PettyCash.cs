using BackEnd.Entities.Auths;
using BackEnd.Enums;

namespace BackEnd.Entities.CashBoxes
{
    public class PettyCash
    {
        public long Id { get; set; }
        public string UserId { get; set; } = null!;
        public User User { get; set; } = null!;
        public double OpeningAmount { get; set; }
        public DateTime OpeningDate { get; set; }
        public double? ClosingAmount { get; set; }
        public double? WithdrawalAmount { get; set; }
        public DateTime? ClosingDate { get; set; }
        public PettyCashStatus Status { get; set; }
        public string? Comments { get; set; }
        public ICollection<PettyCashMovement> PettyCashMovements { get; set; } = [];
    }
}
