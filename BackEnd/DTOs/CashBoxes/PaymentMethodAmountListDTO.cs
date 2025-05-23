using BackEnd.Enums;

namespace BackEnd.DTOs.CashBoxes
{
    public class PaymentMethodAmountListDTO
    {
        public PaymentMethod Method { get; set; }
        public double Amount { get; set; }
    }
}
