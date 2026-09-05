namespace SITRA.Core.DTOs.Invoices.SaleInvoices
{
    public class BalanceSummaryDTO
    {
        public double Expenses { get; set; }
        public double TotalIncome { get; set; }
        public double SpotSales { get; set; }
        public double ReceivablesPayments { get; set; }
        public double Balance { get; set; }
    }
}
