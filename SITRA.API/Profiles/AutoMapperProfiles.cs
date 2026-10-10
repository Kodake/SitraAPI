using AutoMapper;
using SITRA.API.Profiles.Accounts;
using SITRA.API.Profiles.AdministrativeTools;
using SITRA.API.Profiles.Contacts;
using SITRA.API.Profiles.Finances;
using SITRA.API.Profiles.Invoices;
using SITRA.API.Profiles.Products;
using SITRA.API.Profiles.Warehouses;

namespace SITRA.API.Profiles
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            AddProfile<PayableReceiptsProfile>();
            AddProfile<PaymentReceiptsProfile>();
            AddProfile<PayablesProfile>();
            AddProfile<ReceivablesProfile>();
            AddProfile<DeadlinesProfile>();
            AddProfile<FiscalSupportsProfile>();
            AddProfile<RolesProfile>();
            AddProfile<ContactsProfile>();
            AddProfile<CurrenciesProfile>();
            AddProfile<TaxesProfile>();
            AddProfile<ExpensesProfile>();
            AddProfile<InvoicesProfiles>();
            AddProfile<PaymentsProfile>();
            AddProfile<BrandsProfile>();
            AddProfile<CategoriesProfile>();
            AddProfile<ItemsProfile>();
            AddProfile<PricingItemsProfile>();
            AddProfile<PricingsProfile>();
            AddProfile<ProductsProfile>();
            AddProfile<ServicesProfile>();
            AddProfile<StocksProfile>();
            AddProfile<TransfersProfile>();
            AddProfile<WarehousesProfile>();
            AddProfile<QuotationsProfiles>();
        }

        private void AddProfile<TProfile>() where TProfile : Profile, new()
        {
            CreateMap<TProfile, TProfile>();
        }
    }
}
