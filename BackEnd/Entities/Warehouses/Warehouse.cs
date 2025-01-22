using BackEnd.Entities.AdministrativeTools;
using BackEnd.Entities.Invoices;

namespace BackEnd.Entities.Warehouses
{
    public sealed class Warehouse
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? ZipCode { get; set; }
        public bool IsActive { get; set; } = true;
        public List<Stock> Stocks { get; set; } = [];
        public List<FiscalSupport> FiscalSupports { get; set; } = [];
        public List<Transfer> OriginTransfers { get; set; } = [];
        public List<Transfer> DestinationTransfers { get; set; } = [];
    }
}
