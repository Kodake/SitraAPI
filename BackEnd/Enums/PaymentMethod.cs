namespace BackEnd.Enums
{
    public enum PaymentMethod
    {
        Cash = 1,            // Efectivo
        Check = 2,           // Cheque
        Transfer = 3,        // Transferencia
        Deposit = 4,         // Depósito
        DebitCard = 5,       // Tarjeta débito
        CreditCard = 6,      // Tarjeta crédito
        CreditSell = 7,      // Venta a crédito
        Barter = 8,          // Permuta
        GiftVoucher = 9,     // Bonos o certificados de regalos
        Mixed = 10,          // Mixto
        OtherSales = 11      // Otras formas de ventas
    }
}
