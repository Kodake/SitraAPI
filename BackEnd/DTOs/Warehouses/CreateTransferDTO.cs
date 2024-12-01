using BackEnd.Enums;

namespace BackEnd.DTOs.Warehouses
{
    public class CreateTransferDTO
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public long OriginWarehouseId { get; set; }
        public long DestinationWarehouseId { get; set; }
        public double Qty { get; set; }
        public TransferStatus Status { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
