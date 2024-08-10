namespace BackEnd.Entities.Currencies
{
    public class Taxes
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Amount { get; set; }
        public bool IsDeleted { get; set; }

    }
}
