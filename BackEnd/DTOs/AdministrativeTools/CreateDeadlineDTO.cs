namespace BackEnd.DTOs.AdministrativeTools
{
    public class CreateDeadlineDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Days { get; set; }
    }
}
