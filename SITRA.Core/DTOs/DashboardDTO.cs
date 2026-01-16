namespace SITRA.Core.DTOs
{
    public class DashboardDTO
    {
        public int TotalWarehouses { get; set; }
        public int TotalContactsAdded { get; set; }
        public int Last24HoursSaleInvoices { get; set; }
        public double TotalProductsInStock { get; set; }
        public double Last30DaysSales { get; set; }
        public double Last30DaysPurchases { get; set; }
    }
}
