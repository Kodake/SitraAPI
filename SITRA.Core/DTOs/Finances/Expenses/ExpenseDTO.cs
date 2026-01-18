namespace SITRA.Core.DTOs.Finances.Expenses
{
    public class ExpenseDTO
    {
        public long Id { get; set; }
        public DateTimeOffset Date { get; set; }
        public string Description { get; set; } = string.Empty;
        public double Amount { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
