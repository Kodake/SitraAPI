using AutoMapper;
using BackEnd.DTOs.AdministrativeTools;
using BackEnd.DTOs.Contacts;
using BackEnd.DTOs.Finances;
using BackEnd.DTOs.Products;
using BackEnd.DTOs.Warehouses;
using BackEnd.Entities.AdministrativeTools;
using BackEnd.Entities.Contacts;
using BackEnd.Entities.Finances;
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

            CreateMap<CreatePricingDTO, Pricing>();
            CreateMap<Pricing, PricingListDTO>();
            CreateMap<Pricing, PricingDTO>();

            CreateMap<CreatePricingItemDTO, PricingItem>();
            CreateMap<PricingItem, PricingItemListDTO>();
            CreateMap<PricingItem, PricingItemDTO>();

            CreateMap<CreateCurrencyDTO, Currency>();
            CreateMap<Currency, CurrencyListDTO>();
            CreateMap<Currency, CurrencyDTO>();

            CreateMap<CreateTaxDTO, Tax>();
            CreateMap<Tax, TaxListDTO>();
            CreateMap<Tax, TaxDTO>();

            CreateMap<CreateContactDTO, Contact>();
            CreateMap<Contact, ContactListDTO>();
            CreateMap<Contact, ContactDTO>();

            CreateMap<CreateWarehouseDTO, Warehouse>();
            CreateMap<Warehouse, WarehouseListDTO>();
            CreateMap<Warehouse, WarehouseDTO>();

            CreateMap<Item, ProductListDTO>()
                .ForMember(dest => dest.PricingItems, opt => opt.MapFrom(src => src.PricingItems.Select(pt => new PricingItemDTO
                {
                    ItemId = pt.Item.Id,
                    PricingId = pt.Pricing.Id,
                    InitialCost = pt.InitialCost,
                    BasePrice = pt.BasePrice,
                    Percentage = pt.Pricing.Percentage,
                    IsPrincipal = pt.IsPrincipal
                }).ToList()))
                .ForMember(dest => dest.Taxes, opt => opt.MapFrom(src => src.ItemTaxes.Select(pt => new TaxDTO
                {
                    Id = pt.Tax.Id,
                    Name = pt.Tax.Name,
                    Amount = pt.Tax.Amount
                }).ToList()));

            CreateMap<Item, ProductDTO>()
                .ForMember(dest => dest.PricingItems, opt => opt.MapFrom(src => src.PricingItems.Select(pt => new PricingItemDTO
                {
                    ItemId = pt.Item.Id,
                    PricingId = pt.Pricing.Id,
                    InitialCost = pt.InitialCost,
                    BasePrice = pt.BasePrice,
                    Percentage = pt.Pricing.Percentage,
                    IsPrincipal = pt.IsPrincipal
                }).ToList()))
                .ForMember(dest => dest.Taxes, opt => opt.MapFrom(src => src.ItemTaxes.Select(pt => new TaxDTO
                {
                    Id = pt.Tax.Id,
                    Name = pt.Tax.Name,
                    Amount = pt.Tax.Amount
                }).ToList()))
                .ForMember(dest => dest.Brand, opt => opt.MapFrom(src => src.Brand != null ? new BrandDTO
                {
                    Id = src.Brand.Id,
                    Name = src.Brand.Name
                } : null))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category != null ? new CategoryDTO
                {
                    Id = src.Category.Id,
                    Name = src.Category.Name,
                    Description = src.Category.Description
                } : null))
                .ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.Unit != null ? new UnitDTO
                {
                    Id = src.Unit.Id,
                    Name = src.Unit.Name,
                    ShortName = src.Unit.ShortName
                } : null));

            CreateMap<CreateProductDTO, Item>()
                .ForMember(dest => dest.ItemTaxes, opt => opt.MapFrom(src => src.TaxIds.Select(id => new ItemTaxes { TaxId = id }).ToList()))
                .ForMember(dest => dest.Brand, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.Unit, opt => opt.Ignore());

            CreateMap<ProductDTO, Item>();

            CreateMap<Item, ServiceListDTO>()
              .ForMember(dest => dest.Taxes, opt => opt.MapFrom(src => src.ItemTaxes.Select(pt => new TaxDTO
              {
                  Id = pt.Tax.Id,
                  Name = pt.Tax.Name,
                  Amount = pt.Tax.Amount
              }).ToList()));

            CreateMap<Item, ServiceDTO>()
                .ForMember(dest => dest.Taxes, opt => opt.MapFrom(src => src.ItemTaxes.Select(pt => new TaxDTO
                {
                    Id = pt.Tax.Id,
                    Name = pt.Tax.Name,
                    Amount = pt.Tax.Amount
                }).ToList()))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category != null ? new CategoryDTO
                {
                    Id = src.Category.Id,
                    Name = src.Category.Name,
                    Description = src.Category.Description
                } : null))
                .ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.Unit != null ? new UnitDTO
                {
                    Id = src.Unit.Id,
                    Name = src.Unit.Name,
                    ShortName = src.Unit.ShortName
                } : null));

            CreateMap<CreateServiceDTO, Item>()
                .ForMember(dest => dest.ItemTaxes, opt => opt.MapFrom(src => src.TaxIds.Select(id => new ItemTaxes { TaxId = id }).ToList()))
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.Unit, opt => opt.Ignore());

            CreateMap<ServiceDTO, Item>();

            CreateMap<Stock, StockDTO>()
                .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Item))
                .ForMember(dest => dest.Warehouse, opt => opt.MapFrom(src => src.Warehouse))
                .ReverseMap();

            CreateMap<CreateStockDTO, Stock>();

            CreateMap<Stock, StockListDTO>()
                .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Item))
                .ForMember(dest => dest.Warehouse, opt => opt.MapFrom(src => src.Warehouse))
                .ReverseMap();

            CreateMap<FiscalSupport, FiscalSupportDTO>()
                .ForMember(dest => dest.Warehouse, opt => opt.MapFrom(src => src.Warehouse))
                .ReverseMap();

            CreateMap<CreateFiscalSupportDTO, FiscalSupport>();

            CreateMap<FiscalSupport, FiscalSupportListDTO>()
                .ForMember(dest => dest.Warehouse, opt => opt.MapFrom(src => src.Warehouse))
                .ReverseMap();
        }
    }
}
