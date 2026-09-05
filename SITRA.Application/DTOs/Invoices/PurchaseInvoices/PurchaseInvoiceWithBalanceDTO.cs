namespace SITRA.Core.DTOs.Invoices.PurchaseInvoices
{
    public class PurchaseInvoiceWithBalanceDTO
    {
        public List<PurchaseInvoiceListDTO> Invoices { get; set; } = new();
        public PurchaseBalanceSummaryDTO Balance { get; set; } = new();
    }
}
