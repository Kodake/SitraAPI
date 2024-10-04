namespace BackEnd.DTOs.Products
{
    public class CreateProductCsvDTO
    {
        public string Name { get; set; } = string.Empty;
        public string? Reference { get; set; }
        public string? Description { get; set; }
        public double InitialCost { get; set; }
        public double BasePrice { get; set; }
    }
}
