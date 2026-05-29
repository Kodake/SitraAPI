using SITRA.Core.DTOs;
using SITRA.Core.Entities.Products;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.Products
{
    public interface IPricingItemRepository : IRepositoryBase<PricingItem>
    {
        Task<List<PricingItem>> GetAllAsync(PaginationDTO paginationDTO, Guid itemId, CancellationToken cancellationToken = default);
        Task<List<PricingItem>> GetReadOnlyAsync(PaginationDTO paginationDTO, Guid itemId, CancellationToken cancellationToken = default);
        Task<PricingItem?> GetPricingItemByItemIdAndPricingId(Guid itemId, Guid pricingId, CancellationToken cancellationToken = default);
        new Task<bool> ActivateAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(Guid pricingId, Guid itemId, CancellationToken cancellationToken = default);
        Task Delete(Guid id, CancellationToken cancellationToken = default);
    }
}
