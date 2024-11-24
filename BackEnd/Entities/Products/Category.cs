namespace BackEnd.Entities.Products
{
    public sealed class Category
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public ICollection<Item> Items { get; set; } = [];
    }
}
