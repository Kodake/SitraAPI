namespace BackEnd.DTOs.Finances.Currencies;
public class CurrencyDTO
{
    public long Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Rate { get; set; }
}