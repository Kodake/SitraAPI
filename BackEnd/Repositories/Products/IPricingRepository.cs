using BackEnd.Entities.Products;

namespace BackEnd.Repositories.Products
{
    public interface IPricingRepository
    {
        Task<Pricing> Create(Pricing pricing);
        Task<Pricing> GetPricingByName(string? name);
    }
}
