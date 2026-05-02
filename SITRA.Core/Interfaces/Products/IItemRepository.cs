using SITRA.Core.Entities.Products;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.Products
{
    public interface IItemRepository : IRepositoryBase<Item>
    {
        Task<List<Item>> GetByPricingIdAndWarehouseId(Guid pricingId, Guid warehouseId, string? fiscalSupportType = null, CancellationToken cancellationToken = default);
        Task<bool> Exists(Guid id, ItemType itemType, CancellationToken cancellationToken = default);
    }
}
