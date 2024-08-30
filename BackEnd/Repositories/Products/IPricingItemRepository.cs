using BackEnd.Entities.Products;

namespace BackEnd.Repositories.Products
{
    public interface IPricingItemRepository
    {
        Task<int> Create(PricingItem pricingItem);
    }
}
