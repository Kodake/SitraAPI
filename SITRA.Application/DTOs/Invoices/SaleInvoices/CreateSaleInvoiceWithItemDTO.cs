using SITRA.Core.Enums;

namespace SITRA.Core.DTOs.Invoices.SaleInvoices
{
    public class CreateSaleInvoiceWithItemDTO : CreateSaleInvoiceDTO
    {
        public ItemType ItemType { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public double ItemBasePrice { get; set; }
        public List<Guid> TaxIds { get; set; } = [];
    }
}
