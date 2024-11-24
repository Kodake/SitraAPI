namespace BackEnd.DTOs.Finances;
public class TaxListDTO
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Amount { get; set; }
    public bool IsActive { get; set; }
}