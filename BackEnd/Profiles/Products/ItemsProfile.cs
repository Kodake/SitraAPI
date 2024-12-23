using AutoMapper;
using BackEnd.DTOs.Finances.Taxes;
using BackEnd.DTOs.Products.Items;
using BackEnd.DTOs.Products.PricingItems;
using BackEnd.DTOs.Products.Units;
using BackEnd.Entities.Products;

namespace BackEnd.Profiles.Products
{
    public class ItemsProfile : Profile
    {
        public ItemsProfile()
        {
            CreateMap<Item, ItemListDTO>()
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
                .ForMember(des => des.Unit, opt => opt.MapFrom(src => src.Unit != null ? new UnitDTO
                {
                    Id = src.Unit.Id,
                    Name = src.Unit.Name,
                    ShortName = src.Unit.ShortName
                } : null))
                .ForMember(des => des.InitialCost, opt => opt.MapFrom(src =>
                    src.PricingItems.FirstOrDefault(p => p.PricingId == p.PricingId)!.InitialCost))
                .ForMember(des => des.BasePrice, opt => opt.MapFrom(src =>
                    src.PricingItems.FirstOrDefault(p => p.PricingId == p.PricingId)!.BasePrice))
                .ForMember(des => des.Percentage, opt => opt.MapFrom(src =>
                    src.PricingItems.FirstOrDefault(p => p.PricingId == p.PricingId)!.Pricing!.Percentage))
                .ForMember(des => des.TotalPrice, opt => opt.MapFrom(src =>
                    src.PricingItems.FirstOrDefault(p => p.PricingId == p.PricingId)!.BasePrice +
                    src.PricingItems.FirstOrDefault(p => p.PricingId == p.PricingId)!.BasePrice * src.ItemTaxes.Sum(t => t.Tax.Amount) / 100));
        }
    }
}
