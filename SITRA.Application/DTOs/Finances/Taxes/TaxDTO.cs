namespace SITRA.Core.DTOs.Finances.Taxes;
public class TaxDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Amount { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
