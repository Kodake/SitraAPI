namespace BackEnd.Enums
{
    public enum InvoiceStatus
    {
        Draft = 1,
        PendingPayment = 2,
        PartiallyPaid = 3,
        Paid = 4,
        Overdue = 5,
        Cancelled = 6,
        Refunded = 7
    }
}
