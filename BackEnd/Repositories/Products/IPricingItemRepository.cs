using BackEnd.DTOs;
using BackEnd.Entities.Products;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories.Products
{
    public interface IPricingItemRepository
    {
        Task<List<PricingItem>> GetAll(PaginationDTO paginationDTO, long itemId);
        Task<PricingItem?> GetById(long id);
        Task<long> Create(PricingItem pricingItem);
        Task<PricingItem?> GetPricingItemByItemIdAndPricingId(long itemId, long pricingId);
        Task<PricingItem> Update(PricingItem pricingItem);
        Task<bool> Activate(long id);
        Task<bool> Exists(long pricingId, long itemId);
        Task Delete(long id);
    }
}
