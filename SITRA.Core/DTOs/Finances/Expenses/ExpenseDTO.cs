namespace SITRA.Core.DTOs.Finances.Expenses
{
    public class ExpenseDTO
    {
        public long Id { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; } = string.Empty;
        public double Amount { get; set; }
    }
}
