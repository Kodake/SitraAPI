using AutoMapper;
using SITRA.Core.DTOs;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Wizard
{
    public class IsWizardRequiredQuery : IQuery<bool>
    {
        public async Task<bool> GetData(IUnitOfWork unitOfWork, IMapper mapper, CancellationToken cancellationToken)
        {
            // 1. Currency
            var currencies = await unitOfWork.CurrencyRepository.GetCurrencies();

            // 2. Tax
            var taxes = await unitOfWork.TaxRepository.GetAll(StatusType.Activated);

            // 3. Warehouse
            var warehouses = await unitOfWork.WarehouseRepository.GetAll(StatusType.Activated);

            // 4. Product (Item)
            var products = await unitOfWork.ProductRepository.GetAllAsync(new PaginationDTO { RecordsPerPage = 1 });

            // 5. Pricing
            var pricingName = Enum.GetName(typeof(PricingName), PricingName.Principal)?.ToUpper() ?? "PRINCIPAL";
            var pricing = await unitOfWork.PricingRepository.GetPricingByName(pricingName);

            // Returns true only if EVERYTHING is missing
            return !currencies.Any() && !taxes.Any() && !warehouses.Any() && !products.Any() && pricing == null;
        }
    }
}
