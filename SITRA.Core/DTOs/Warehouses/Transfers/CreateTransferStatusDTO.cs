namespace SITRA.Core.DTOs.Warehouses.Transfers
{
    public class CreateTransferStatusDTO
    {
        public Guid Id { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
