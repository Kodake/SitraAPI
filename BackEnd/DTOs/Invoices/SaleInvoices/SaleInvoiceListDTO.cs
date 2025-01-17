using BackEnd.Enums;

namespace BackEnd.DTOs.Invoices.SaleInvoices
{
    public class SaleInvoiceListDTO
    {
        public long Id { get; set; }
        public DateTime Date { get; set; }
        public string FiscalSupportNumber { get; set; } = string.Empty;
        public double? Shipping { get; set; } = null!;
        public double? Discount { get; set; } = null!;
        public double TotalQtyItems { get; set; }
        public double TotalItems { get; set; }
        public double TotalAmount { get; set; }
        public double TotalPrice { get; set; }
        public double TotalTaxes { get; set; }
        public InvoiceCondition ConditionType { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public List<SaleInvoiceItemDTO> SaleInvoiceItems { get; set; } = [];
    }
}
