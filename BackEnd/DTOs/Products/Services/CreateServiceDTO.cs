namespace BackEnd.DTOs.Products.Services
{
    public class CreateServiceDTO
    {
        public string Name { get; set; } = string.Empty;
        public string? Reference { get; set; }
        public string? Description { get; set; }
        public double InitialCost { get; set; } = 0;
        public double BasePrice { get; set; }
        public List<int> TaxIds { get; set; } = [];
        public int? CategoryId { get; set; }
        public int? SubCategoryId { get; set; }
        public int? UnitId { get; set; }
    }
}
