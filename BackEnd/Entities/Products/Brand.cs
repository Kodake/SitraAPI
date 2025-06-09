namespace BackEnd.Entities.Products
{
    public sealed class Brand
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ImageURL { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public ICollection<Item> Items { get; set; } = [];
        public ICollection<Model> Models { get; set; } = [];
    }
}
