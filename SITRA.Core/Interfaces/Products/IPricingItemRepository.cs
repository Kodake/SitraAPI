using SITRA.Core.DTOs;
using SITRA.Core.Entities.Products;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.Products
{
    public interface IPricingItemRepository : IRepositoryBase<PricingItem>
    {
        Task<List<PricingItem>> GetAllAsync(PaginationDTO paginationDTO, long itemId);
        Task<PricingItem?> GetPricingItemByItemIdAndPricingId(long itemId, long pricingId);
        new Task<bool> ActivateAsync(long id);
        Task<bool> ExistsAsync(long pricingId, long itemId);
        Task Delete(long id);
    }
}
