using SITRA.Core.Interfaces;
using AutoMapper;
using SITRA.Core.Enums;
using SITRA.Core.DTOs;
using SITRA.Core.Mediator.Queries;

namespace SITRA.Core.Mediator.Queries.Wizard
{
    public class IsWizardRequiredQuery : IQuery<bool>
    {
        public async Task<bool> GetData(IUnitOfWork unitOfWork, IMapper mapper, CancellationToken cancellationToken)
        {
            // 1. Currency
            var currencies = await unitOfWork.CurrencyRepository.GetCurrencies();
            if (!currencies.Any()) return true;

            // 2. Tax
            var taxes = await unitOfWork.TaxRepository.GetAll(StatusType.Activated);
            if (!taxes.Any()) return true;

            // 3. Warehouse
            var warehouses = await unitOfWork.WarehouseRepository.GetAll(StatusType.Activated);
            if (!warehouses.Any()) return true;

            // 4. Product (Item)
            var products = await unitOfWork.ProductRepository.GetAllAsync(new PaginationDTO { RecordsPerPage = 1 });
            if (!products.Any()) return true;

            // 5. Pricing
            var pricingName = Enum.GetName(typeof(PricingName), PricingName.Principal)?.ToUpper() ?? "PRINCIPAL";
            var pricing = await unitOfWork.PricingRepository.GetPricingByName(pricingName);
            if (pricing == null) return true;

            return false;
        }
    }
}
