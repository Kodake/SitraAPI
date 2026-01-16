namespace SITRA.Core.DTOs.Finances.Taxes;
public class TaxDTO
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Amount { get; set; }
}
