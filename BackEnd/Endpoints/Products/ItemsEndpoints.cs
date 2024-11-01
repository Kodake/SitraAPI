using AutoMapper;
using BackEnd.DTOs.Finances;
using BackEnd.DTOs.Products;
using BackEnd.DTOs.Warehouses;
using BackEnd.Repositories.Products;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BackEnd.Endpoints
{
    public static class ItemsEndpoints
    {
        public static RouteGroupBuilder MapItems(this RouteGroupBuilder group)
        {
            group.MapGet("/pricing/{pricingId:int}/warehouse/{warehouseId:int}/", GetByPricingIdAndWarehouseId);
            return group;
        }

        static async Task<Ok<List<ItemListDTO>>> GetByPricingIdAndWarehouseId(IItemRepository repository,
            IMapper mapper, int pricingId, int warehouseId)
        {
            var products = await repository.GetByPricingIdAndWarehouseId(pricingId, warehouseId);
            var productsDTO = products.Select(item =>
            {
                // Find the relevant PricingItem based on the given pricingId
                var pricingItem = item.PricingItems.FirstOrDefault(pi => pi.PricingId == pricingId);

                // Create the DTO and populate fields
                return new ItemListDTO
                {
                    Id = item.Id,
                    Name = item.Name,
                    Reference = item.Reference,
                    ItemType = item.ItemType,
                    InitialCost = pricingItem?.InitialCost ?? 0,
                    BasePrice = pricingItem?.BasePrice ?? 0,
                    Percentage = pricingItem?.Pricing.Percentage ?? 0,
                    TotalPrice = pricingItem != null ? pricingItem.BasePrice * (1 + (pricingItem.Pricing.Percentage / 100)) : 0,
                    IsActive = item.IsActive,
                    Unit = item.Unit != null ? new UnitDTO
                    {
                        Id = item.Unit.Id,
                        Name = item.Unit.Name,
                        ShortName = item.Unit.ShortName
                    } : null,
                    Taxes = item.ItemTaxes.Select(it => new TaxDTO
                    {
                        Id = it.Tax.Id,
                        Name = it.Tax.Name,
                        Amount = it.Tax.Amount
                    }).ToList(),
                    Stocks = item.Stocks.Select(s => new StockDTO
                    {
                    }).ToList()
                };
            }).ToList();

            return TypedResults.Ok(productsDTO);
        }
    }
}