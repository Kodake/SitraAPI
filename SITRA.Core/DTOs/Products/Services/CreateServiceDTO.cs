using Microsoft.AspNetCore.Http;
namespace SITRA.Core.DTOs.Products.Services
{
    public class CreateServiceDTO
    {
        public string Name { get; set; } = string.Empty;
        public string? Reference { get; set; }
        public string? Description { get; set; }
        public IFormFile? ImageUrl { get; set; }
        public double InitialCost { get; set; } = 0;
        public double BasePrice { get; set; }
        public List<Guid> TaxIds { get; set; } = [];
        public Guid? CategoryId { get; set; }
        public Guid? SubCategoryId { get; set; }
        public Guid? UnitId { get; set; }
    }
}

