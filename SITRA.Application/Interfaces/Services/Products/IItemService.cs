using SITRA.Core.Entities.Products;

namespace SITRA.Core.Interfaces.Services.Products
{
    public interface IItemService
    {
        Task<List<Item>> GetValidItemsForPricingAndWarehouseAsync(Guid pricingId, Guid warehouseId, string? fiscalSupportType = null, CancellationToken cancellationToken = default);
    }
}
