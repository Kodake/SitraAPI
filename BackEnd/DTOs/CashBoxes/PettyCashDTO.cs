using BackEnd.DTOs.Auths;
using BackEnd.Enums;

namespace BackEnd.DTOs.CashBoxes
{
    public class PettyCashDTO
    {
        public long Id { get; set; }
        public string UserId { get; set; } = null!;
        public UserDTO User { get; set; } = null!;
        public double OpeningAmount { get; set; }
        public DateTime OpeningDate { get; set; }
        public double? ClosingAmount { get; set; }
        public DateTime? ClosingDate { get; set; }
        public PettyCashStatus Status { get; set; }
        public string? Comments { get; set; }
        public ICollection<PettyCashMovementListDTO> PettyCashMovements { get; set; } = [];
    }
}
