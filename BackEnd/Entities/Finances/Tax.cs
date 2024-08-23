using BackEnd.Entities.Products;

namespace BackEnd.Entities.Finances
{
    public sealed class Tax
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Amount { get; set; }
        public bool IsActive { get; set; } = true;
        public ICollection<ItemTaxes> ItemTaxes { get; set; } = [];
    }
}
