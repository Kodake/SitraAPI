namespace BackEnd.DTOs.Currencies;
public class TaxListDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Amount { get; set; }
    public bool IsActive { get; set; }
}