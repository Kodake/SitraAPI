namespace SITRA.Core.DTOs.Finances.Expenses
{
    public class CreateExpenseDTO
    {
        public DateTime Date { get; set; }
        public string Description { get; set; } = string.Empty;
        public double Amount { get; set; }
    }
}
