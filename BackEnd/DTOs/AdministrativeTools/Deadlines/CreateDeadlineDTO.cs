namespace BackEnd.DTOs.AdministrativeTools.Deadlines
{
    public class CreateDeadlineDTO
    {
        public string Name { get; set; } = string.Empty;
        public long Days { get; set; }
    }
}
