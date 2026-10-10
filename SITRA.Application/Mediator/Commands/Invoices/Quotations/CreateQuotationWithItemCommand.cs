using AutoMapper;
using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.DTOs.Invoices.SaleInvoices;
using SITRA.Core.Entities.Invoices;
using SITRA.Core.Entities.Products;
using SITRA.Core.Entities.Warehouses;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Commands.Invoices.Quotations
{
    public class CreateQuotationWithItemCommand : CreateCommand<Quotation, CreateQuotationWithItemDTO>, IIdempotentCommand
    {
        public CreateQuotationWithItemCommand(CreateQuotationWithItemDTO dto) : base(dto) { }

        public override async Task<CreateQuotationWithItemDTO>ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper, CancellationToken cancellationToken)
        {
            // ── Step 1: Ensure Principal Pricing exists ─┃
            var pricing = await unitOfWork.PricingRepository.GetPricingByName(Enum.GetName(PricingName.Principal));
            if (pricing == null)
            {
                pricing = new Pricing
                {
                    Name = Enum.GetName(PricingName.Principal)?.ToUpper() ?? "PRINCIPAL",
                    PricingType = PricingType.Value,
                    IsDefault = true
                };
                pricing = await unitOfWork.PricingRepository.CreateAsync(pricing);
            }

            if (Dto.PricingId == Guid.Empty)
            {
                Dto.PricingId = pricing.Id;
            }

            if (Dto.WarehouseId == Guid.Empty)
            {
                var warehouses = await unitOfWork.WarehouseRepository.GetAll(StatusType.Activated);
                var defWarehouse = warehouses.FirstOrDefault(w => w.IsDefault) ?? warehouses.FirstOrDefault();
                if (defWarehouse != null)
                {
                    Dto.WarehouseId = defWarehouse.Id;
                }
            }

            // If InvoiceItems is empty, fallback to the root DTO item
            if (Dto.InvoiceItems.Count == 0)
            {
                Dto.InvoiceItems.Add(new SaleInvoiceItemDTO
                {
                    ItemId = Guid.Empty,
                    ItemType = Dto.ItemType,
                    ItemName = Dto.ItemName,
                    ItemBasePrice = Dto.ItemBasePrice,
                    TaxIds = Dto.TaxIds,
                    Qty = 1,
                    CreatedAt = DateTimeOffset.Now
                });
            }

            // ── Step 2: Create any new items (ItemId == Guid.Empty) on the fly ─┃
            foreach (var itemDTO in Dto.InvoiceItems)
            {
                if (itemDTO.ItemId == Guid.Empty)
                {
                    string itemName = !string.IsNullOrWhiteSpace(itemDTO.ItemName) ? itemDTO.ItemName : Dto.ItemName;
                    double basePrice = itemDTO.ItemBasePrice ?? Dto.ItemBasePrice;
                    var taxIds = itemDTO.TaxIds ?? Dto.TaxIds ?? [];

                    var item = new Item
                    {
                        Name = itemName,
                        ItemType = itemDTO.ItemType,
                        ItemTaxes = taxIds.Select(taxId => new ItemTaxes { TaxId = taxId }).ToList()
                    };

                    Item savedItem;
                    if (itemDTO.ItemType == ItemType.Product)
                    {
                        savedItem = await unitOfWork.ProductRepository.CreateAsync(item);
                    }
                    else
                    {
                        savedItem = await unitOfWork.ServiceRepository.CreateAsync(item);
                    }

                    var pricingItem = new PricingItem
                    {
                        Item = savedItem,
                        Pricing = pricing,
                        InitialCost = 0,
                        BasePrice = basePrice,
                        IsPrincipal = true
                    };
                    await unitOfWork.PricingItemRepository.CreateAsync(pricingItem);

                    if (itemDTO.ItemType == ItemType.Product)
                    {
                        var warehouse = await unitOfWork.WarehouseRepository.GetByIdAsync(Dto.WarehouseId);
                        var stock = new Stock
                        {
                            Item = savedItem,
                            Qty = 0,
                            MinQty = 0,
                            MaxQty = 0,
                            WarehouseId = Dto.WarehouseId,
                            Warehouse = warehouse!
                        };
                        await unitOfWork.StockRepository.CreateAsync(stock);
                    }

                    itemDTO.ItemId = savedItem.Id;
                }
            }

            // Commit new items, pricing items and stocks so they can be queried during total/item calculations
            await unitOfWork.SaveChangesAsync();

            // ── Step 3: Create Quotation and SaleInvoice ─┃
            var currencyDB = await unitOfWork.CurrencyRepository.GetByIdAsync(Dto.CurrencyId);

            var saleInvoice = mapper.Map<Invoice>(Dto);
            saleInvoice.Date = DateTimeOffset.Now;
            saleInvoice.InvoiceType = InvoiceType.Quotation;
            if (Dto.CustomerId.HasValue && Dto.CustomerId.Value != Guid.Empty)
            {
                saleInvoice.BuyerId = Dto.CustomerId.Value;
            }

            if (Dto.BillerId.HasValue && Dto.BillerId.Value != Guid.Empty)
            {
                saleInvoice.BillerId = Dto.BillerId.Value;
            }

            double currencyRate = currencyDB!.Rate;
            await CalculateInvoiceTotals(Dto.InvoiceItems, saleInvoice, unitOfWork, currencyRate);

            await unitOfWork.SaleInvoiceRepository.CreateAsync(saleInvoice);

            var quotation = new Quotation
            {
                Invoice = saleInvoice,
                ContactId = Dto.CustomerId.HasValue && Dto.CustomerId.Value != Guid.Empty ? Dto.CustomerId.Value : null,
                QuotationNumber = Dto.QuotationNumber,
            };

            await unitOfWork.QuotationRepository.CreateAsync(quotation);

            await AddInvoiceItems(Dto.InvoiceItems, saleInvoice, unitOfWork, mapper, currencyRate);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("quotations-get", cancellationToken);

            return Dto;
        }

        private static async Task CalculateInvoiceTotals(
            IEnumerable<SaleInvoiceItemDTO> items,
            Invoice saleInvoice,
            IUnitOfWork unitOfWork,
            double currencyRate)
        {
            double totalQtyItems = 0, totalItems = 0, totalAmount = 0, totalPrice = 0, totalTaxes = 0;

            foreach (var itemDTO in items)
            {
                switch (itemDTO.ItemType)
                {
                    case ItemType.Product:
                        var product = await unitOfWork.ProductRepository.GetByIdAsync(itemDTO.ItemId);

                        var productBasePrice = product!.PricingItems.First(x => x.ItemId == itemDTO.ItemId).BasePrice / currencyRate;
                        var productTaxAmount = product.ItemTaxes.Sum(x => x?.Tax?.Amount) ?? 0;

                        totalQtyItems += itemDTO.Qty;
                        totalItems += 1;
                        totalAmount += productBasePrice * itemDTO.Qty;
                        totalTaxes += productBasePrice * productTaxAmount / 100 * itemDTO.Qty;
                        break;

                    case ItemType.Service:
                        var service = await unitOfWork.ServiceRepository.GetByIdAsync(itemDTO.ItemId);

                        var serviceBasePrice = service!.PricingItems.First(x => x.ItemId == itemDTO.ItemId).BasePrice / currencyRate;
                        var serviceTaxAmount = service.ItemTaxes.Sum(x => x?.Tax?.Amount) ?? 0;

                        totalQtyItems += itemDTO.Qty;
                        totalItems += 1;
                        totalAmount += serviceBasePrice * itemDTO.Qty;
                        totalTaxes += serviceBasePrice * serviceTaxAmount / 100 * itemDTO.Qty;
                        break;

                    case ItemType.Combo:
                        var combo = await unitOfWork.ComboRepository.GetByIdAsync(itemDTO.ItemId);

                        var comboBasePrice = combo!.PricingItems.First(x => x.ItemId == itemDTO.ItemId).BasePrice / currencyRate;
                        var comboTaxAmount = combo.ItemTaxes.Sum(x => x?.Tax?.Amount) ?? 0;

                        totalQtyItems += itemDTO.Qty;
                        totalItems += 1;
                        totalAmount += comboBasePrice * itemDTO.Qty;
                        totalTaxes += (comboBasePrice * comboTaxAmount / 100) * itemDTO.Qty;
                        break;

                    default:
                        break;
                }
            }

            totalPrice = totalAmount + totalTaxes;
            saleInvoice.TotalQtyItems = totalQtyItems;
            saleInvoice.TotalItems = totalItems;
            saleInvoice.TotalPrice = saleInvoice.Discount.HasValue && saleInvoice.Discount > 0
                ? (double)(totalPrice + (saleInvoice.Shipping ?? 0)
                   - ((totalPrice + (saleInvoice.Shipping ?? 0)) * saleInvoice.Discount.Value / 100))
                : totalPrice + (saleInvoice.Shipping ?? 0);
            saleInvoice.TotalAmount = totalAmount;
            saleInvoice.TotalTaxes = totalTaxes;
        }

        private static async Task AddInvoiceItems(
            IEnumerable<SaleInvoiceItemDTO> items,
            Invoice saleInvoice,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            double currencyRate)
        {
            foreach (var itemDTO in items)
            {
                var saleInvoiceItem = saleInvoice.InvoiceItems.FirstOrDefault(i => i.ItemId == itemDTO.ItemId);
                if (saleInvoiceItem == null)
                {
                    saleInvoiceItem = mapper.Map<InvoiceItem>(itemDTO);
                    saleInvoice.InvoiceItems.Add(saleInvoiceItem);
                }

                saleInvoiceItem.InvoiceId = saleInvoice.Id;
                saleInvoiceItem.Invoice = saleInvoice;

                var (basePrice, taxAmount) = await GetItemPricingAndTax(itemDTO, unitOfWork, currencyRate);

                saleInvoiceItem.TotalAmount = basePrice * itemDTO.Qty;
                saleInvoiceItem.TotalTaxes = basePrice * taxAmount / 100 * itemDTO.Qty;
                saleInvoiceItem.TotalPrice = saleInvoiceItem.TotalAmount + saleInvoiceItem.TotalTaxes;
                saleInvoiceItem.BasePrice = basePrice;

                await unitOfWork.SaleInvoiceRepository.AddInvoiceItem(saleInvoiceItem);
            }
        }

        private static async Task<(double basePrice, double taxAmount)> GetItemPricingAndTax(
            SaleInvoiceItemDTO itemDTO,
            IUnitOfWork unitOfWork,
            double currencyRate)
        {
            var item = await unitOfWork.ItemRepository.GetByIdAsync(itemDTO.ItemId);

            var basePrice = item!.PricingItems.First(x => x.ItemId == itemDTO.ItemId).BasePrice / currencyRate;
            var taxAmount = item.ItemTaxes.Sum(x => x?.Tax?.Amount) ?? 0;

            return (basePrice, taxAmount);
        }
    }
}
