namespace BackEnd.DTOs.Finances;
public class CreateCurrencyDTO
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Rate { get; set; } = 0;
}