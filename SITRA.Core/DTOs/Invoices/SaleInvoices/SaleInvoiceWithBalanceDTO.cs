using SITRA.Core.DTOs;

namespace SITRA.Core.DTOs.Invoices.SaleInvoices
{
    public class SaleInvoiceWithBalanceDTO
    {
        public List<SaleInvoiceListDTO> Invoices { get; set; } = new();
        public BalanceSummaryDTO Balance { get; set; } = new();
    }
}
