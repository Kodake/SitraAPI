using SITRA.Core.Entities.AdministrativeTools;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Entities.Warehouses
{
    public sealed class Warehouse : ITenant, IAuditableEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? ZipCode { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public Guid TenantId { get; set; }
        public List<Stock> Stocks { get; set; } = [];
        public List<FiscalSupport> FiscalSupports { get; set; } = [];
        public List<Transfer> OriginTransfers { get; set; } = [];
        public List<Transfer> DestinationTransfers { get; set; } = [];
    }
}
