namespace BackEnd.Entities.Products
{
    public class ComboItem
    {
        public long ParentItemId { get; set; }
        public Item ParentItem { get; set; } = null!;
        public long ChildItemId { get; set; }
        public Item ChildItem { get; set; } = null!;
        public double Qty { get; set; }
    }
}
