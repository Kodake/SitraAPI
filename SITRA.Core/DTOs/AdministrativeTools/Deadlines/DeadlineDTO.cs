namespace SITRA.Core.DTOs.AdministrativeTools.Deadlines
{
    public class DeadlineDTO
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public long Days { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
