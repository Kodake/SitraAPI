using BackEnd.Enums;

namespace BackEnd.DTOs.Invoices.PurchaseInvoices
{
    public class PurchaseInvoiceListDTO
    {
        public long Id { get; set; }
        public DateTime Date { get; set; }
        public string? FiscalSupportNumber { get; set; } = string.Empty;
        public double? Discount { get; set; } = null!;
        public double TotalQtyItems { get; set; }
        public double TotalItems { get; set; }
        public double TotalAmount { get; set; }
        public double TotalPrice { get; set; }
        public double TotalTaxes { get; set; }
        public InvoiceCondition ConditionType { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public List<PurchaseInvoiceItemDTO> InvoiceItems { get; set; } = [];
    }
}
