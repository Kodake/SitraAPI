using BackEnd.Enums;

namespace BackEnd.DTOs.Invoices
{
    public class CreateSaleInvoiceDTO
    {
        public int PricingId { get; set; }
        public int CurrencyId { get; set; }
        public int WarehouseId { get; set; }
        public int CustomerId { get; set; }
        public int BillerId { get; set; }
        public DocumentType DocumentType { get; set; }
        public int FiscalSupportId { get; set; }
        public double? Shipping { get; set; }
        public string? Description { get; set; }
        public string? Note { get; set; }
    }
}
