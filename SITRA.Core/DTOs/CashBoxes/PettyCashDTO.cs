using SITRA.Core.Entities.Auths;
using SITRA.Core.DTOs.Auths;
using SITRA.Core.Enums;

namespace SITRA.Core.DTOs.CashBoxes
{
    public class PettyCashDTO
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = null!;
        public UserDTO User { get; set; } = null!;
        public double OpeningAmount { get; set; }
        public DateTimeOffset OpeningDate { get; set; }
        public double ClosingAmount { get; set; }
        public double WithdrawalAmount { get; set; }
        public DateTimeOffset? ClosingDate { get; set; }
        public PettyCashStatus Status { get; set; }
        public string? Comments { get; set; }
        public List<PaymentMethodAmountListDTO> PaymentMethods { get; set; } = [];
        public double CashBilled { get; set; }
        public double TotalBilled { get; set; }
    }
}

