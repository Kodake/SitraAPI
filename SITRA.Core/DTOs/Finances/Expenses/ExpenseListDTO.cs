namespace SITRA.Core.DTOs.Finances.Expenses
{
    public class ExpenseListDTO
    {
        public long Id { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; } = string.Empty;
        public double Amount { get; set; }
        public bool IsActive { get; set; }
    }
}
