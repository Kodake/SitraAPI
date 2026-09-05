using SITRA.Core.Enums;

namespace SITRA.Core.DTOs.Finances.Taxes;
public class CreateTaxDTO
{
    public string Name { get; set; } = string.Empty;
    public double Amount { get; set; } = 0;
    public TaxType TaxType { get; set; }
}
