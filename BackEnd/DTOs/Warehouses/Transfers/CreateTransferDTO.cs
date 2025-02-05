namespace BackEnd.DTOs.Warehouses.Transfers
{
    public class CreateTransferDTO
    {
        public long ProductId { get; set; }
        public long OriginWarehouseId { get; set; }
        public long DestinationWarehouseId { get; set; }
        public double Qty { get; set; }
        //public TransferStatus Status { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
