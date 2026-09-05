namespace SITRA.Core.DTOs.Products.Services
{
    public class CreateServiceCsvDTO
    {
        public string Name { get; set; } = string.Empty;
        public string? Reference { get; set; }
        public string? Description { get; set; }
        public double BasePrice { get; set; }
    }
}
