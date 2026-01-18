namespace SITRA.Core.DTOs.Finances.Taxes;
public class TaxListDTO
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Amount { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
