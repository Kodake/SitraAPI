using SITRA.Core.Enums;

namespace SITRA.Core.DTOs.Finances.Taxes;
public class TaxListDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Amount { get; set; }
    public TaxType TaxType { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
