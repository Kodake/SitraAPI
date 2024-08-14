namespace BackEnd.DTOs.Finances;
public class CreateTaxDTO
{
    public string Name { get; set; } = string.Empty;
    public double Amount { get; set; } = 0;
}