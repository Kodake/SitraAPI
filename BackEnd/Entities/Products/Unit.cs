namespace BackEnd.Entities.Products
{
    public sealed class Unit
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public ICollection<Item> Items { get; set; } = [];
    }
}
