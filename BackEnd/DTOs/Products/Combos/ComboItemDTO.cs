namespace BackEnd.DTOs.Products.Combos
{
    public class ComboItemDTO
    {
        public long ParentItemId { get; set; }
        public ProductDTO ParentItem { get; set; } = null!;
        public long ChildItemId { get; set; }
        public ProductDTO ChildItem { get; set; } = null!;
    }
}
