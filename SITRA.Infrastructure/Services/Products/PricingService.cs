using SITRA.Core.Entities.Products;
using SITRA.Core.Interfaces;
using SITRA.Core.Interfaces.Services.Products;

namespace SITRA.Infrastructure.Services.Products
{
    public class PricingService : IPricingService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PricingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Pricing>> GetUnregisteredPricingsForItemAsync(Guid itemId, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.PricingRepository.GetAllAsync(p => !p.PricingItems.Any(pi => pi.ItemId == itemId), cancellationToken);
        }

        public async Task<bool> SetDefaultAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var pricing = await _unitOfWork.PricingRepository.GetByIdAsync(id, cancellationToken);
            if (pricing == null) return false;

            var defaults = await _unitOfWork.PricingRepository.GetAllAsync(x => x.IsDefault && x.Id != id, cancellationToken);

            foreach (var p in defaults)
            {
                p.IsDefault = false;
                _unitOfWork.PricingRepository.Update(p);
            }

            pricing.IsDefault = true;
            _unitOfWork.PricingRepository.Update(pricing);
            
            return true;
        }
    }
}
