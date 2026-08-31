using AutoMapper;
using SITRA.Core.DTOs.Finances.Taxes;
using SITRA.Core.DTOs.Products.Brands;
using SITRA.Core.DTOs.Products.Categories;
using SITRA.Core.DTOs.Products.Combos;
using SITRA.Core.DTOs.Products.Models;
using SITRA.Core.DTOs.Products.PricingItems;
using SITRA.Core.DTOs.Products.SubCategories;
using SITRA.Core.Entities.Products;

namespace SITRA.API.Profiles.Products
{
    public class CombosProfile : Profile
    {
        public CombosProfile()
        {
            CreateMap<CreateComboDTO, Item>()
                .ForMember(des => des.ItemTaxes, opt => opt.MapFrom(src => src.TaxIds.Select(id => new ItemTaxes { TaxId = id }).ToList()))
                .ForMember(des => des.ComboItems, opt => opt.MapFrom(src =>
                    src.Items.Select(i => new ComboItem
                    {
                        ChildItemId = i.Id,
                        Qty = i.Qty
                    }).ToList()));

            CreateMap<ComboDTO, Item>();
            CreateMap<Item, ComboListDTO>()
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

            CreateMap<Item, ComboDTO>()
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
                .ForMember(des => des.ComboItems, opt => opt.MapFrom(src => src.ComboItems))
                .ForMember(des => des.Brand, opt => opt.MapFrom(src => src.Brand != null ? new BrandDTO
                {
                    Id = src.Brand.Id,
                    Name = src.Brand.Name
                } : null))
                .ForMember(des => des.Model, opt => opt.MapFrom(src => src.Model != null ? new ModelDTO
                {
                    Id = src.Model.Id,
                    Name = src.Model.Name,
                    Description = src.Model.Description
                } : null))
                .ForMember(des => des.Category, opt => opt.MapFrom(src => src.Category != null ? new CategoryDTO
                {
                    Id = src.Category.Id,
                    Name = src.Category.Name,
                    Description = src.Category.Description
                } : null))
                .ForMember(des => des.SubCategory, opt => opt.MapFrom(src => src.SubCategory != null ? new SubCategoryDTO
                {
                    Id = src.SubCategory.Id,
                    Name = src.SubCategory.Name,
                    Description = src.SubCategory.Description
                } : null))
                .ForMember(des => des.InitialCost, opt => opt.MapFrom(src =>
                    src.PricingItems.FirstOrDefault(p => p.IsPrincipal)!.InitialCost))
                .ForMember(des => des.BasePrice, opt => opt.MapFrom(src =>
                    src.PricingItems.FirstOrDefault(p => p.IsPrincipal)!.BasePrice))
                .ForMember(des => des.Percentage, opt => opt.MapFrom(src =>
                    src.PricingItems.FirstOrDefault(p => p.IsPrincipal)!.Pricing!.Percentage))
                .ForMember(des => des.TaxIds, opt => opt.MapFrom(src => src.ItemTaxes.Select(pt => pt.Tax.Id).ToList()))
                .ForMember(des => des.Items, opt => opt.MapFrom(src =>
                    src.ComboItems.Select(ci => new ItemQtyDTO
                    {
                        Id = (Guid)ci.ChildItemId,
                        Qty = ci.Qty
                    }).ToList()));

            CreateMap<ComboItem, ComboItemDTO>()
                .ForMember(des => des.Id, opt => opt.MapFrom(src => src.ChildItem.Id))
                .ForMember(des => des.Name, opt => opt.MapFrom(src => src.ChildItem.Name));
        }
    }
}

