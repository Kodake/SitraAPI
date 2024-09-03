using BackEnd.DTOs;
using BackEnd.Entities.Products;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories.Products
{
    public interface IPricingItemRepository
    {
        Task<List<PricingItem>> GetAll(PaginationDTO paginationDTO, int itemId);
        Task<int> Create(PricingItem pricingItem);
        Task<PricingItem?> GetPricingItemByItemIdAndPricingId(int itemId, int pricingId);
        Task<PricingItem> Update(PricingItem pricingItem);
        Task<bool> Activate(int id);
        Task<bool> Exists(int pricingId, int itemId);
    }
}
