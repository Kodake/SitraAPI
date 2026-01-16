using SITRA.Core.Entities.Products;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.Products
{
    public interface IItemRepository : IRepositoryBase<Item>
    {
        Task<List<Item>> GetByPricingIdAndWarehouseId(long pricingId, long warehouseId);
        Task<bool> Exists(long id, ItemType itemType);
    }
}
