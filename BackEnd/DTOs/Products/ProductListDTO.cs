using BackEnd.DTOs.Currencies;
using BackEnd.Entities.Products;

namespace BackEnd.DTOs.Products;
public class ProductListDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double InitialCost { get; set; }
    public double BasePrice { get; set; }
    public double TotalPrice
    {
        get
        {
            double totalTaxes = Taxes.Sum(pt => pt.Amount);
            return BasePrice + (BasePrice * totalTaxes / 100);
        }
    }
    public bool IsActive { get; set; }
    public List<TaxDTO> Taxes { get; set; } = new List<TaxDTO>();
}