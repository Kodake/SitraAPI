namespace BackEnd.DTOs.Products.Units
{
    public class UnitListDTO
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
