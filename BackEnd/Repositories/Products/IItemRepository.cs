using BackEnd.Entities.Products;
using BackEnd.Enums;
using BackEnd.Repositories.Generics;

namespace BackEnd.Repositories.Products
{
    public interface IItemRepository : IRepositoryBase<Item>
    {
        Task<List<Item>> GetByPricingIdAndWarehouseId(long pricingId, long warehouseId);
        Task<bool> Exists(long id, ItemType itemType);
    }
}
