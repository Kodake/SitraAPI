using SITRA.Core.Interfaces;

namespace SITRA.Core.Entities.AdministrativeTools
{
    public sealed class Deadline : ITenant
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Days { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public Guid TenantId { get; set; }
    }
}
