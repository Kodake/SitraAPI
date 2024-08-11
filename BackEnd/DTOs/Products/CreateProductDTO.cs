namespace BackEnd.DTOs.Products
{
    public class CreateProductDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Reference { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double InitialCost { get; set; }
        public double BasePrice { get; set; }
        public List<int> TaxIds { get; set; } = new();
        public int? BrandId { get; set; }
        public int? CategoryId { get; set; }
        public int? UnitId { get; set; }
    }
}
