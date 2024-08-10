namespace BackEnd.Entities.Products
{
    public class Unit
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
        public ICollection<Product> Products { get; set; } = [];
    }
}
