namespace BackEnd.DTOs.Invoices.SaleInvoices
{
    public class SaleInvoiceDTO
    {
        public long Id { get; set; }
        public DateTime Date { get; set; }
        public DateTime? DueDate { get; set; }
        public string FiscalSupportNumber { get; set; } = string.Empty;
    }
}
