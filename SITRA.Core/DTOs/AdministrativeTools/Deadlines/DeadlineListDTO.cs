namespace SITRA.Core.DTOs.AdministrativeTools.Deadlines
{
    public class DeadlineListDTO
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public long Days { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
