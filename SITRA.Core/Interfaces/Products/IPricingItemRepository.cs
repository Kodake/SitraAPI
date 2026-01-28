using SITRA.Core.DTOs;
using SITRA.Core.Entities.Products;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.Products
{
    public interface IPricingItemRepository : IRepositoryBase<PricingItem>
    {
        Task<List<PricingItem>> GetAllAsync(PaginationDTO paginationDTO, Guid itemId);
        Task<PricingItem?> GetPricingItemByItemIdAndPricingId(Guid itemId, Guid pricingId);
        new Task<bool> ActivateAsync(Guid id);
        Task<bool> ExistsAsync(Guid pricingId, Guid itemId);
        Task Delete(Guid id);
    }
}
