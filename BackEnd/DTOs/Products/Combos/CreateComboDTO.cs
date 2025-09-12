namespace BackEnd.DTOs.Products.Combos
{
    public class CreateComboDTO
    {
        public List<ItemQtyDTO> Items { get; set; } = null!;
        public string Name { get; set; } = string.Empty;
        public string? Reference { get; set; }
        public string? Barcode { get; set; }
        public string? Description { get; set; }
        public double InitialCost { get; set; }
        public double BasePrice { get; set; }
        public List<long> TaxIds { get; set; } = null!;
        public long? BrandId { get; set; }
        public int? ModelId { get; set; }
        public long? CategoryId { get; set; }
        public int? SubCategoryId { get; set; }
        public long? UnitId { get; set; }
    }
}
