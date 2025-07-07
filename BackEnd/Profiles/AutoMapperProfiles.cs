using AutoMapper;
using BackEnd.Profiles.Accounts;
using BackEnd.Profiles.AdministrativeTools;
using BackEnd.Profiles.Contacts;
using BackEnd.Profiles.Finances;
using BackEnd.Profiles.Invoices;
using BackEnd.Profiles.Products;
using BackEnd.Profiles.Warehouses;

namespace BackEnd.Profiles
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
            AddProfile<UnitsProfile>();
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
