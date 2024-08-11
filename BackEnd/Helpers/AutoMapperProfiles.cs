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

            CreateMap<Product, ProductListDTO>()
                .ForMember(dest => dest.Taxes, opt => opt.MapFrom(src => src.ProductTaxes.Select(pt => new TaxDTO
                {
                    Id = pt.Tax.Id,
                    Name = pt.Tax.Name,
                    Amount = pt.Tax.Amount
                }).ToList()));

            CreateMap<Product, ProductDTO>()
                .ForMember(dest => dest.Taxes, opt => opt.MapFrom(src => src.ProductTaxes.Select(pt => new TaxDTO
                {
                    Id = pt.Tax.Id,
                    Name = pt.Tax.Name,
                    Amount = pt.Tax.Amount
                }).ToList()));

            CreateMap<CreateProductDTO, Product>()
                .ForMember(dest => dest.ProductTaxes, opt => opt.MapFrom(src => src.TaxIds.Select(id => new ProductTaxes { TaxId = id }).ToList()))
                .ForMember(dest => dest.Brand, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.Unit, opt => opt.Ignore());

            CreateMap<Product, ProductDTO>()
                .ForMember(dest => dest.Taxes, opt => opt.MapFrom(src => src.ProductTaxes.Select(pt => pt.TaxId).ToList()))
                .ForMember(dest => dest.BrandId, opt => opt.MapFrom(src => src.BrandId))
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
                .ForMember(dest => dest.UnitId, opt => opt.MapFrom(src => src.UnitId));

            CreateMap<ProductDTO, Product>();


            //CreateMap<Product, ProductListDTO>()
            //    .ForMember(dest => dest.ProductTaxes, opt => opt.MapFrom(src => src.ProductTaxes.Select(pt => pt.TaxId).ToList()));

            //CreateMap<CreateProductDTO, Product>()
            //    .ForMember(dest => dest.ProductTaxes, opt => opt.MapFrom(src => src.TaxIds.Select(taxId => new ProductTaxes { TaxId = taxId }).ToList()));
        }
    }
}
