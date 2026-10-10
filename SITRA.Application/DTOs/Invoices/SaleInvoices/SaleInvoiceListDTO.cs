using SITRA.Core.DTOs.Contacts.Contacts;
using SITRA.Core.Enums;

namespace SITRA.Core.DTOs.Invoices.SaleInvoices
{
    public class SaleInvoiceListDTO
    {
        public Guid Id { get; set; }
        public DateTimeOffset Date { get; set; }
        public string FiscalSupportNumber { get; set; } = string.Empty;
        public Guid? ContactId { get; set; }
        public virtual ContactDTO? Buyer { get; set; } = null!;
        public double? Shipping { get; set; } = null!;
        public double? Discount { get; set; } = null!;
        public double TotalQtyItems { get; set; }
        public double TotalItems { get; set; }
        public double TotalAmount { get; set; }
        public double TotalPrice { get; set; }
        public double TotalTaxes { get; set; }
        public InvoiceCondition ConditionType { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public List<SaleInvoiceItemDTO> InvoiceItems { get; set; } = [];
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
