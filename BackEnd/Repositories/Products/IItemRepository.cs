using BackEnd.Entities.Products;

namespace BackEnd.Repositories.Products
{
    public interface IItemRepository
    {
        Task<List<Item>> GetByPricingIdAndWarehouseId(int pricingId, int warehouseId);
    }
}
