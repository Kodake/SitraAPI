using AutoMapper;
using SITRA.Core.DTOs.Finances.Taxes;
using SITRA.Core.DTOs.Products.Categories;
using SITRA.Core.DTOs.Products.PricingItems;
using SITRA.Core.DTOs.Products.Services;
using SITRA.Core.DTOs.Products.SubCategories;
using SITRA.Core.DTOs.Products.Units;
using SITRA.Core.Entities.Products;

namespace SITRA.API.Profiles.Products
{
    public class ServicesProfile : Profile
    {
        public ServicesProfile()
        {
            CreateMap<CreateServiceDTO, Item>()
                .ForMember(des => des.ItemTaxes, opt => opt.MapFrom(src => src.TaxIds.Select(id => new ItemTaxes { TaxId = id }).ToList()))
                .ForMember(des => des.ItemTaxes, opt => opt.MapFrom(src => src.TaxIds.Select(id => new ItemTaxes { TaxId = id }).ToList()));

            CreateMap<CreateServiceCsvDTO, Item>();

            CreateMap<ServiceDTO, Item>();

            CreateMap<Item, ServiceDTO>()
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
                .ForMember(des => des.Category, opt => opt.MapFrom(src => src.ItemCategory != null ? new CategoryDTO
                {
                    Id = src.ItemCategory.Category.Id,
                    Name = src.ItemCategory.Category.Name,
                    Description = src.ItemCategory.Category.Description
                } : null))
                .ForMember(des => des.SubCategory, opt => opt.MapFrom(src => src.ItemSubCategory != null ? new SubCategoryDTO
                {
                    Id = src.ItemSubCategory.SubCategory.Id,
                    Name = src.ItemSubCategory.SubCategory.Name,
                    Description = src.ItemSubCategory.SubCategory.Description
                } : null))
                .ForMember(des => des.Unit, opt => opt.MapFrom(src => src.ItemUnit != null ? new UnitDTO
                {
                    Id = src.ItemUnit.Unit.Id,
                    Name = src.ItemUnit.Unit.Name,
                    ShortName = src.ItemUnit.Unit.ShortName
                } : null))
                .ForMember(des => des.BasePrice, opt => opt.MapFrom(src =>
                    src.PricingItems.FirstOrDefault(p => p.IsPrincipal)!.BasePrice))
                .ForMember(des => des.Percentage, opt => opt.MapFrom(src =>
                    src.PricingItems.FirstOrDefault(p => p.IsPrincipal)!.Pricing!.Percentage))
                .ForMember(des => des.TaxIds, opt => opt.MapFrom(src => src.ItemTaxes.Select(pt => pt.Tax.Id).ToList()));

            CreateMap<Item, ServiceListDTO>()
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

