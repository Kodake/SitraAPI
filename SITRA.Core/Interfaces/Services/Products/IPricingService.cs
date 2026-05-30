using SITRA.Core.Entities.Products;

namespace SITRA.Core.Interfaces.Services.Products
{
    public interface IPricingService
    {
        Task<IEnumerable<Pricing>> GetUnregisteredPricingsForItemAsync(Guid itemId, CancellationToken cancellationToken = default);
        Task<bool> SetDefaultAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
