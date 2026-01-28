using AutoMapper;
using SITRA.Core.DTOs.Finances.Taxes;
using SITRA.Core.DTOs.Products.Brands;
using SITRA.Core.DTOs.Products.Categories;
using SITRA.Core.DTOs.Products.Combos;
using SITRA.Core.DTOs.Products.Models;
using SITRA.Core.DTOs.Products.PricingItems;
using SITRA.Core.DTOs.Products.SubCategories;
using SITRA.Core.DTOs.Products.Units;
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
                    }).ToList()))
                .ForMember(des => des.ItemBrand, opt => opt.MapFrom(src => src.BrandId.HasValue ? new ItemBrand { BrandId = src.BrandId.Value } : null))
                .ForMember(des => des.ItemModel, opt => opt.MapFrom(src => src.ModelId.HasValue ? new ItemModel { ModelId = src.ModelId.Value } : null))
                .ForMember(des => des.ItemCategory, opt => opt.MapFrom(src => src.CategoryId.HasValue ? new ItemCategory { CategoryId = src.CategoryId.Value } : null))
                .ForMember(des => des.ItemSubCategory, opt => opt.MapFrom(src => src.SubCategoryId.HasValue ? new ItemSubCategory { SubCategoryId = src.SubCategoryId.Value } : null))
                .ForMember(des => des.ItemUnit, opt => opt.MapFrom(src => src.UnitId.HasValue ? new ItemUnit { UnitId = src.UnitId.Value } : null));

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
                .ForMember(des => des.Brand, opt => opt.MapFrom(src => src.ItemBrand != null ? new BrandDTO
                {
                    Id = src.ItemBrand.Brand.Id,
                    Name = src.ItemBrand.Brand.Name
                } : null))
                .ForMember(des => des.BrandId, opt => opt.MapFrom(src => src.ItemBrand != null ? (Guid)src.ItemBrand.Brand.Id : (Guid?)null))
                .ForMember(des => des.Model, opt => opt.MapFrom(src => src.ItemModel != null ? new ModelDTO
                {
                    Id = src.ItemModel.Model.Id,
                    Name = src.ItemModel.Model.Name,
                    Description = src.ItemModel.Model.Description
                } : null))
                .ForMember(des => des.ModelId, opt => opt.MapFrom(src => src.ItemModel != null ? (Guid)src.ItemModel.Model.Id : (Guid?)null))
                .ForMember(des => des.Category, opt => opt.MapFrom(src => src.ItemCategory != null ? new CategoryDTO
                {
                    Id = src.ItemCategory.Category.Id,
                    Name = src.ItemCategory.Category.Name,
                    Description = src.ItemCategory.Category.Description
                } : null))
                .ForMember(des => des.CategoryId, opt => opt.MapFrom(src => src.ItemCategory != null ? (Guid)src.ItemCategory.Category.Id : (Guid?)null))
                .ForMember(des => des.SubCategory, opt => opt.MapFrom(src => src.ItemSubCategory != null ? new SubCategoryDTO
                {
                    Id = src.ItemSubCategory.SubCategory.Id,
                    Name = src.ItemSubCategory.SubCategory.Name,
                    Description = src.ItemSubCategory.SubCategory.Description
                } : null))
                .ForMember(des => des.SubCategoryId, opt => opt.MapFrom(src => src.ItemSubCategory != null ? (Guid)src.ItemSubCategory.SubCategory.Id : (Guid?)null))
                .ForMember(des => des.Unit, opt => opt.MapFrom(src => src.ItemUnit != null ? new UnitDTO
                {
                    Id = src.ItemUnit.Unit.Id,
                    Name = src.ItemUnit.Unit.Name,
                    ShortName = src.ItemUnit.Unit.ShortName
                } : null))
                .ForMember(des => des.UnitId, opt => opt.MapFrom(src => src.ItemUnit != null ? (Guid)src.ItemUnit.Unit.Id : (Guid?)null))
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

