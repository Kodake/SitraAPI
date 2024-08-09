using AutoMapper;
using BackEnd.DTOs.Contacts;
using BackEnd.DTOs.Currencies;
using BackEnd.DTOs.Products;
using BackEnd.DTOs.Warehouses;
using BackEnd.Entities.Contacts;
using BackEnd.Entities.Currencies;
using BackEnd.Entities.Products;
using BackEnd.Entities.Warehouses;

namespace BackEnd.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<CreateCategoryDTO, Category>();
            CreateMap<Category, CategoryListDTO>();
            CreateMap<Category, CategoryDTO>();

            CreateMap<CreateBrandDTO, Brand>();
            CreateMap<Brand, BrandListDTO>();
            CreateMap<Brand, BrandDTO>();

            CreateMap<CreateUnitDTO, Unit>();
            CreateMap<Unit, UnitListDTO>();
            CreateMap<Unit, UnitDTO>();

            CreateMap<CreateCurrencyDTO, Currency>();
            CreateMap<Currency, CurrencyListDTO>();
            CreateMap<Currency, CurrencyDTO>();

            CreateMap<CreateContactDTO, Contact>();
            CreateMap<Contact, ContactListDTO>();
            CreateMap<Contact, ContactDTO>();

            CreateMap<CreateWarehouseDTO, Warehouse>();
            CreateMap<Warehouse, WarehouseListDTO>();
            CreateMap<Warehouse, WarehouseDTO>();
        }
    }
}
