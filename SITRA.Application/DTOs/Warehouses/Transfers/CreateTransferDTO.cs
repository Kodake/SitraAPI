namespace SITRA.Core.DTOs.Warehouses.Transfers
{
    public class CreateTransferDTO
    {
        public Guid ProductId { get; set; }
        public Guid OriginWarehouseId { get; set; }
        public Guid DestinationWarehouseId { get; set; }
        public double Qty { get; set; }
        //public TransferStatus Status { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
