using AutoMapper;
using BackEnd.DTOs.Finances.Taxes;
using BackEnd.DTOs.Products.Brands;
using BackEnd.DTOs.Products.Categories;
using BackEnd.DTOs.Products.PricingItems;
using BackEnd.DTOs.Products.Products;
using BackEnd.DTOs.Products.Units;
using BackEnd.Entities.Products;

namespace BackEnd.Profiles.Products
{
    public class ProductsProfile : Profile
    {
        public ProductsProfile()
        {
            CreateMap<CreateProductDTO, Item>()
                .ForMember(des => des.ItemTaxes, opt => opt.MapFrom(src => src.TaxIds.Select(id => new ItemTaxes { TaxId = id }).ToList()))
                .ForMember(des => des.Brand, opt => opt.Ignore())
                .ForMember(des => des.Category, opt => opt.Ignore())
                .ForMember(des => des.Unit, opt => opt.Ignore());

            CreateMap<CreateProductCsvDTO, Item>();

            CreateMap<ProductDTO, Item>();

            CreateMap<Item, ProductDTO>()
                .ForMember(des => des.PricingItems, opt => opt.MapFrom(src => src.PricingItems.Select(pt => new PricingItemDTO
                {
                    ItemId = pt.Item.Id,
                    PricingId = pt.Pricing.Id,
                    InitialCost = pt.InitialCost,
                    BasePrice = pt.BasePrice,
                    Percentage = pt.Pricing.Percentage,
                    IsPrincipal = pt.IsPrincipal
                }).ToList()))
                .ForMember(des => des.Taxes, opt => opt.MapFrom(src => src.ItemTaxes.Select(pt => new TaxDTO
                {
                    Id = pt.Tax.Id,
                    Name = pt.Tax.Name,
                    Amount = pt.Tax.Amount
                }).ToList()))
                .ForMember(des => des.Brand, opt => opt.MapFrom(src => src.Brand != null ? new BrandDTO
                {
                    Id = src.Brand.Id,
                    Name = src.Brand.Name
                } : null))
                .ForMember(des => des.Category, opt => opt.MapFrom(src => src.Category != null ? new CategoryDTO
                {
                    Id = src.Category.Id,
                    Name = src.Category.Name,
                    Description = src.Category.Description
                } : null))
                .ForMember(des => des.Unit, opt => opt.MapFrom(src => src.Unit != null ? new UnitDTO
                {
                    Id = src.Unit.Id,
                    Name = src.Unit.Name,
                    ShortName = src.Unit.ShortName
                } : null))
                .ForMember(des => des.InitialCost, opt => opt.MapFrom(src =>
                    src.PricingItems.FirstOrDefault(p => p.IsPrincipal)!.InitialCost))
                .ForMember(des => des.BasePrice, opt => opt.MapFrom(src =>
                    src.PricingItems.FirstOrDefault(p => p.IsPrincipal)!.BasePrice))
                .ForMember(des => des.Percentage, opt => opt.MapFrom(src =>
                    src.PricingItems.FirstOrDefault(p => p.IsPrincipal)!.Pricing!.Percentage))
                .ForMember(des => des.TaxIds, opt => opt.MapFrom(src => src.ItemTaxes.Select(pt => pt.Tax.Id).ToList()));

            CreateMap<Item, ProductListDTO>()
               .ForMember(des => des.PricingItems, opt => opt.MapFrom(src => src.PricingItems.Select(pt => new PricingItemDTO
               {
                   ItemId = pt.Item.Id,
                   PricingId = pt.Pricing.Id,
                   InitialCost = pt.InitialCost,
                   BasePrice = pt.BasePrice,
                   Percentage = pt.Pricing.Percentage,
                   IsPrincipal = pt.IsPrincipal
               }).ToList()))
               .ForMember(des => des.Taxes, opt => opt.MapFrom(src => src.ItemTaxes.Select(pt => new TaxDTO
               {
                   Id = pt.Tax.Id,
                   Name = pt.Tax.Name,
                   Amount = pt.Tax.Amount
               }).ToList()))
               .ForMember(des => des.InitialCost, opt => opt.MapFrom(src =>
                   src.PricingItems.FirstOrDefault(p => p.IsPrincipal)!.InitialCost))
               .ForMember(des => des.BasePrice, opt => opt.MapFrom(src =>
                   src.PricingItems.FirstOrDefault(p => p.IsPrincipal)!.BasePrice))
               .ForMember(des => des.Percentage, opt => opt.MapFrom(src =>
                   src.PricingItems.FirstOrDefault(p => p.IsPrincipal)!.Pricing!.Percentage))
               .ForMember(des => des.TotalPrice, opt => opt.MapFrom(src =>
                   src.PricingItems.FirstOrDefault(p => p.IsPrincipal)!.BasePrice +
                   src.PricingItems.FirstOrDefault(p => p.IsPrincipal)!.BasePrice * src.ItemTaxes.Sum(t => t.Tax.Amount) / 100
               ));
        }
    }
}
