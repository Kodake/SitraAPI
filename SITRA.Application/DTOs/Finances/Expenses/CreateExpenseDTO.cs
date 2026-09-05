namespace SITRA.Core.DTOs.Finances.Expenses
{
    public class CreateExpenseDTO
    {
        public DateTimeOffset Date { get; set; }
        public string Description { get; set; } = string.Empty;
        public double Amount { get; set; }
    }
}
