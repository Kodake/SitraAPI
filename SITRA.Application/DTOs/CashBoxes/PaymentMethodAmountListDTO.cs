using SITRA.Core.Enums;

namespace SITRA.Core.DTOs.CashBoxes
{
    public class PaymentMethodAmountListDTO
    {
        public PaymentMethod MethodType { get; set; }
        public double Amount { get; set; }
    }
}
