namespace SITRA.Core.DTOs.Finances.Currencies;
public class CreateCurrencyDTO
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public double Rate { get; set; } = 0;
}
