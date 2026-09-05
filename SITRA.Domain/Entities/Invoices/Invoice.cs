using SITRA.Core.Entities.Accounts;
using SITRA.Core.Entities.AdministrativeTools;
using SITRA.Core.Entities.Contacts;
using SITRA.Core.Entities.Finances;
using SITRA.Core.Entities.Products;
using SITRA.Core.Entities.Warehouses;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Entities.Invoices
{
    public class Invoice : ITenant, IAuditableEntity
    {
        public Guid Id { get; set; }
        public DateTimeOffset Date { get; set; }
        public Guid PricingId { get; set; }
        public Pricing Pricing { get; set; } = null!;
        public Guid? BillerId { get; set; }
        public virtual Contact? Biller { get; set; }
        public Guid? BuyerId { get; set; }
        public virtual Contact? Buyer { get; set; }
        public Guid? FiscalSupportId { get; set; }
        public virtual FiscalSupport? FiscalSupport { get; set; }
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
        public string? DgiiTrackId { get; set; }
        public string? DgiiStatus { get; set; }
        public string? DgiiResponse { get; set; }
        public string? SecurityCode { get; set; }
        public string? XmlUrl { get; set; }
        public Guid TenantId { get; set; }
        public Receivable? Receivable { get; set; } = null!;
        public Payable? Payable { get; set; } = null!;
        public SpotPayment? SpotPayment { get; set; } = null!;
        public SpotPayable? SpotPayable { get; set; } = null!;
        public Quotation? Quotation { get; set; } = null!;
        public List<InvoiceItem> InvoiceItems { get; set; } = [];
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public decimal? MontoNoFacturable { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public decimal? SaldoAnterior { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public decimal? MontoAvancePago { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public decimal? MontoPeriodo { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string? TipoIngresos { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public int? IndicadorEnvioDiferido { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string? IndicadorMontoGral { get; set; }
        // Credit/Debit note reference fields (E33/E34)
        public string? ModifiedNcfNumber { get; set; }
        public int? ModificationCode { get; set; }
    }
}
