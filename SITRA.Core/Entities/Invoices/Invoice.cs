using SITRA.Core.Interfaces;
using SITRA.Core.Entities.Accounts;
using SITRA.Core.Entities.AdministrativeTools;
using SITRA.Core.Entities.Contacts;
using SITRA.Core.Entities.Finances;
using SITRA.Core.Entities.Products;
using SITRA.Core.Entities.Warehouses;
using SITRA.Core.Enums;

namespace SITRA.Core.Entities.Invoices
{
    public class Invoice : ITenant, IAuditableEntity
    {
        public Guid Id { get; set; }
        public DateTimeOffset Date { get; set; }
        public Guid PricingId { get; set; }
        public Pricing Pricing { get; set; } = null!;
        public virtual InvoiceBiller? InvoiceBiller { get; set; }
        public virtual InvoiceBuyer? InvoiceBuyer { get; set; }
        public virtual InvoiceFiscalSupport? InvoiceFiscalSupport { get; set; }
        public string? FiscalSupportNumber { get; set; } = null!;
        public Guid CurrencyId { get; set; }
        public Currency Currency { get; set; } = null!;
        public Guid WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; } = null!;
        public InvoiceCondition? ConditionType { get; set; }
        public InvoiceType InvoiceType { get; set; }
        public double TotalQtyItems { get; set; }
        public double TotalItems { get; set; }
        public double TotalAmount { get; set; }
        public double TotalPrice { get; set; }
        public double TotalTaxes { get; set; }
        public double? Shipping { get; set; } = null!;
        public double? Discount { get; set; } = null!;
        public string? Description { get; set; } = null!;
        public string? Note { get; set; } = null!;
        public DateTimeOffset? DueDate { get; set; } = null!;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public Guid TenantId { get; set; }
        public Receivable? Receivable { get; set; } = null!;
        public Payable? Payable { get; set; } = null!;
        public SpotPayment? SpotPayment { get; set; } = null!;
        public SpotPayable? SpotPayable { get; set; } = null!;
        public Quotation? Quotation { get; set; } = null!;
        public List<InvoiceItem> InvoiceItems { get; set; } = [];
    }
}
