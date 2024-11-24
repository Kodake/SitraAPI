using BackEnd.Enums;

namespace BackEnd.DTOs.Invoices
{
    public class SaleInvoiceItemDTO
    {
        public long ItemId { get; set; }
        public ItemType ItemType { get; set; }
        public double Qty { get; set; }
    }
}
