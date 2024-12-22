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
            AddProfile<AccountsProfile>();
            AddProfile<AdministrativeToolsProfile>();
            AddProfile<ContactsProfile>();
            AddProfile<FinancesProfile>();
            AddProfile<InvoicesProfiles>();
            AddProfile<ProductsProfile>();
            AddProfile<WarehousesProfile>();
        }

        private void AddProfile<TProfile>() where TProfile : Profile, new()
        {
            CreateMap<TProfile, TProfile>();
        }
    }
}
