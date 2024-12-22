namespace BackEnd.Entities.AdministrativeTools
{
    public sealed class Deadline
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Days { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
