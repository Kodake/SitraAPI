using BackEnd.Entities.Products;

namespace BackEnd.Repositories.Products
{
    public class PricingRepository : IPricingRepository
    {
        public Task<Pricing> Create(Pricing pricing)
        {
            throw new NotImplementedException();
        }

        public Task<Pricing> GetPricingByName(string? name)
        {
            throw new NotImplementedException();
        }
    }
}
