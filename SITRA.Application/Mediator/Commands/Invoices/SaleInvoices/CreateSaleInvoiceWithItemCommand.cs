using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.DTOs.Invoices.SaleInvoices;
using SITRA.Core.Entities.Accounts;
using SITRA.Core.Entities.CashBoxes;
using SITRA.Core.Entities.Invoices;
using SITRA.Core.Entities.Products;
using SITRA.Core.Entities.Warehouses;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Commands.Invoices.SaleInvoices
{
    public class CreateSaleInvoiceWithItemCommand(CreateSaleInvoiceWithItemDTO dto, string? modifiedNcfNumber = null)
        : CreateCommand<Invoice, CreateSaleInvoiceWithItemDTO>(dto), IIdempotentCommand
    {
        public string? ModifiedNcfNumber { get; } = modifiedNcfNumber;

        public Func<Invoice, Task<(bool isAccepted, string trackId, string status, string securityCode, string rawResponse)>>? DgiiProcess { get; set; }

        public override async Task<CreateSaleInvoiceWithItemDTO> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper, CancellationToken cancellationToken)
        {
            // ── Step 1: Ensure Principal Pricing exists ──
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

            // ── Step 2: Create any new items (ItemId == Guid.Empty) on the fly ──
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
                            Warehouse = warehouse!
                        };
                        await unitOfWork.StockRepository.CreateAsync(stock);
                    }

                    itemDTO.ItemId = savedItem.Id;
                    itemDTO.ItemTaxes = savedItem.ItemTaxes;
                }
            }

            // Save to generate real Item IDs for all newly created items
            await unitOfWork.SaveChangesAsync();

            // ── Step 5: Execute the Sale Invoice creation (same logic as CreateSaleInvoiceCommand) ──
            var user = await unitOfWork.UserService.GetUserAsync();
            var pettyCash = await unitOfWork.PettyCashRepository.GetOpenPettyCashAsync();

            var fiscalSupportDB = Dto.FiscalSupportId.HasValue
                ? await unitOfWork.FiscalSupportRepository.GetByIdAsync(Dto.FiscalSupportId.Value)
                : null;

            var currencyDB = await unitOfWork.CurrencyRepository.GetByIdAsync(Dto.CurrencyId);

            var saleInvoice = mapper.Map<Invoice>(Dto as CreateSaleInvoiceDTO);
            saleInvoice.InvoiceType = InvoiceType.Sale;
            saleInvoice.Date = DateTimeOffset.Now;
            saleInvoice.ModifiedNcfNumber = ModifiedNcfNumber;
            if (!string.IsNullOrEmpty(ModifiedNcfNumber))
                saleInvoice.ModificationCode = 3;

            if (Dto.CustomerId.HasValue && Dto.CustomerId.Value != Guid.Empty)
            {
                saleInvoice.BuyerId = Dto.CustomerId.Value;
            }

            if (Dto.BillerId.HasValue && Dto.BillerId.Value != Guid.Empty)
            {
                saleInvoice.BillerId = Dto.BillerId.Value;
            }

            if (fiscalSupportDB != null)
            {
                fiscalSupportDB.IncrementSequence();
                saleInvoice.FiscalSupportNumber = fiscalSupportDB.GenerateFiscalSupportNumber();
                var ecfTypeKey = saleInvoice.FiscalSupportNumber?.Length >= 3
                    ? saleInvoice.FiscalSupportNumber.Substring(0, 3)
                    : null;
                if (Enum.TryParse<EcfType>(ecfTypeKey, ignoreCase: true, out var parsedType)
                    && parsedType != EcfType.E31
                    && parsedType != EcfType.E33
                    && parsedType != EcfType.E34
                    && parsedType != EcfType.E41
                    && parsedType != EcfType.B01
                    && parsedType != EcfType.B03
                    && parsedType != EcfType.B04
                    && parsedType != EcfType.B11)
                {
                    saleInvoice.FiscalSupportId = fiscalSupportDB.Id;
                    unitOfWork.FiscalSupportRepository.Update(fiscalSupportDB);
                }
            }

            double currencyRate = currencyDB!.Rate;
            await CalculateInvoiceTotals(Dto.InvoiceItems, saleInvoice, unitOfWork, currencyRate);

            await unitOfWork.SaleInvoiceRepository.CreateAsync(saleInvoice);
            await AddInvoiceItems(Dto.InvoiceItems, saleInvoice, unitOfWork, mapper, currencyRate);

            await unitOfWork.SaveChangesAsync();

            // Perform DGII Workflow
            if (DgiiProcess != null && !string.IsNullOrEmpty(saleInvoice.FiscalSupportNumber))
            {
                var fullInvoice = await unitOfWork.SaleInvoiceRepository.GetByIdAsync(saleInvoice.Id, cancellationToken);
                fullInvoice!.ModifiedNcfNumber = saleInvoice.ModifiedNcfNumber;
                fullInvoice.ModificationCode = saleInvoice.ModificationCode;

                var dgiiResult = await DgiiProcess(fullInvoice!);

                saleInvoice.DgiiTrackId = string.IsNullOrEmpty(dgiiResult.trackId) ? null : dgiiResult.trackId;
                saleInvoice.DgiiStatus = dgiiResult.status;
                saleInvoice.DgiiResponse = dgiiResult.rawResponse;
                saleInvoice.SecurityCode = dgiiResult.securityCode;
                saleInvoice.Status = dgiiResult.isAccepted
                    ? InvoiceStatus.Active
                    : (dgiiResult.status == "Error" ? InvoiceStatus.Failed : InvoiceStatus.Rejected);

                unitOfWork.SaleInvoiceRepository.Update(saleInvoice);
                await unitOfWork.SaveChangesAsync();

                if (!dgiiResult.isAccepted)
                {
                    Dto.Id = saleInvoice.Id;
                    return Dto;
                }
            }

            var payments = Dto.Payments.Select(p => mapper.Map<Payment>(p)).ToList();

            switch (Dto.ConditionType)
            {
                case InvoiceCondition.Credit:
                    var accountsReceivable = new Receivable
                    {
                        Invoice = saleInvoice,
                        ContactId = Dto.CustomerId == Guid.Empty ? null : Dto.CustomerId,
                        TotalAmountDue = saleInvoice.TotalPrice,
                        TotalOutstanding = 0,
                        Status = AccountStatus.PendingReceivable,
                    };

                    await unitOfWork.ReceivableRepository.CreateAsync(accountsReceivable);
                    break;

                case InvoiceCondition.Spot:
                    var spotPayment = new SpotPayment
                    {
                        Invoice = saleInvoice,
                        ContactId = Dto.CustomerId == Guid.Empty ? null : Dto.CustomerId,
                        AmountReturned = Dto.AmountReturned ?? 0,
                        PaymentDate = DateTimeOffset.UtcNow,
                        IsActive = true
                    };

                    await unitOfWork.SpotPaymentRepository.CreateAsync(spotPayment);

                    foreach (var payment in payments)
                    {
                        payment.SpotPayment = spotPayment;
                        await unitOfWork.PaymentRepository.CreateAsync(payment);
                    }
                    break;
            }

            await ReduceStock(Dto, unitOfWork);
            if (pettyCash != null)
            {
                await AddPettyCashMovement(unitOfWork, pettyCash, saleInvoice);
            }
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("fiscal-supports-get", cancellationToken);
            await outputCacheStore.EvictByTagAsync("stocks-get", cancellationToken);
            await outputCacheStore.EvictByTagAsync("receivables-get", cancellationToken);
            await outputCacheStore.EvictByTagAsync("spotPayments-get", cancellationToken);
            await outputCacheStore.EvictByTagAsync("pettyCashMovements-get", cancellationToken);
            await outputCacheStore.EvictByTagAsync("products-get", cancellationToken);
            await outputCacheStore.EvictByTagAsync("services-get", cancellationToken);
            await outputCacheStore.EvictByTagAsync("pricing-items-get", cancellationToken);
            await outputCacheStore.EvictByTagAsync("pricings-get", cancellationToken);
            Dto.Id = saleInvoice.Id;

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
                        if (product != null)
                        {
                            var pricingItem = product.PricingItems?.FirstOrDefault(x => x.ItemId == itemDTO.ItemId);
                            var productBasePrice = (pricingItem?.BasePrice ?? 0) / currencyRate;
                            var productTaxAmount = product.ItemTaxes?.Sum(x => x?.Tax?.Amount) ?? 0;

                            totalQtyItems += itemDTO.Qty;
                            totalItems += 1;
                            totalAmount += productBasePrice * itemDTO.Qty;
                            totalTaxes += (productBasePrice * productTaxAmount / 100) * itemDTO.Qty;
                        }
                        break;

                    case ItemType.Service:
                        var service = await unitOfWork.ServiceRepository.GetByIdAsync(itemDTO.ItemId);
                        if (service != null)
                        {
                            var pricingItem = service.PricingItems?.FirstOrDefault(x => x.ItemId == itemDTO.ItemId);
                            var serviceBasePrice = (pricingItem?.BasePrice ?? 0) / currencyRate;
                            var serviceTaxAmount = service.ItemTaxes?.Sum(x => x?.Tax?.Amount) ?? 0;

                            totalQtyItems += itemDTO.Qty;
                            totalItems += 1;
                            totalAmount += serviceBasePrice * itemDTO.Qty;
                            totalTaxes += (serviceBasePrice * serviceTaxAmount / 100) * itemDTO.Qty;
                        }
                        break;

                    case ItemType.Combo:
                        var combo = await unitOfWork.ComboRepository.GetByIdAsync(itemDTO.ItemId);
                        if (combo != null)
                        {
                            var pricingItem = combo.PricingItems?.FirstOrDefault(x => x.ItemId == itemDTO.ItemId);
                            var comboBasePrice = (pricingItem?.BasePrice ?? 0) / currencyRate;
                            var comboTaxAmount = combo.ItemTaxes?.Sum(x => x?.Tax?.Amount) ?? 0;

                            totalQtyItems += itemDTO.Qty;
                            totalItems += 1;
                            totalAmount += comboBasePrice * itemDTO.Qty;
                            totalTaxes += (comboBasePrice * comboTaxAmount / 100) * itemDTO.Qty;
                        }
                        break;

                    default:
                        break;
                }
            }

            totalPrice = totalAmount + totalTaxes;
            saleInvoice.TotalQtyItems = totalQtyItems;
            saleInvoice.TotalItems = totalItems;
            saleInvoice.TotalPrice = saleInvoice.Discount.HasValue && saleInvoice.Discount > 0
                ? (double)((totalPrice + (saleInvoice.Shipping ?? 0))
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
                bool isNew = false;
                if (saleInvoiceItem == null)
                {
                    saleInvoiceItem = mapper.Map<InvoiceItem>(itemDTO);
                    saleInvoice.InvoiceItems.Add(saleInvoiceItem);
                    isNew = true;
                }

                saleInvoiceItem.InvoiceId = saleInvoice.Id;
                saleInvoiceItem.Invoice = saleInvoice;

                var (basePrice, taxAmount) = await GetItemPricingAndTax(itemDTO, unitOfWork, currencyRate);

                saleInvoiceItem.TotalAmount = basePrice * itemDTO.Qty;
                saleInvoiceItem.TotalTaxes = basePrice * taxAmount / 100 * itemDTO.Qty;
                saleInvoiceItem.TotalPrice = saleInvoiceItem.TotalAmount + saleInvoiceItem.TotalTaxes;
                saleInvoiceItem.BasePrice = basePrice;

                if (isNew)
                {
                    await unitOfWork.SaleInvoiceRepository.AddInvoiceItem(saleInvoiceItem);
                }
            }
        }

        private static async Task ReduceStock(CreateSaleInvoiceDTO dto, IUnitOfWork unitOfWork)
        {
            var stockAdjustments = new Dictionary<Guid, double>();

            foreach (var item in dto.InvoiceItems)
            {
                if (item.ItemType == ItemType.Product)
                {
                    var stock = await unitOfWork.StockRepository.GetByItemIdAndWarehouseId(item.ItemId, dto.WarehouseId);

                    if (stock != null)
                    {
                        if (!stockAdjustments.ContainsKey(stock.Id))
                            stockAdjustments[stock.Id] = 0;

                        stockAdjustments[stock.Id] -= item.Qty;
                    }
                }

                if (item.ItemType == ItemType.Combo)
                {
                    var combo = await unitOfWork.ComboRepository.GetByIdAsync(item.ItemId);

                    if (combo?.ComboItems != null)
                    {
                        foreach (var comboItem in combo.ComboItems)
                        {
                            var stock = await unitOfWork.StockRepository
                                .GetByItemIdAndWarehouseId(comboItem.ChildItemId, dto.WarehouseId);

                            if (stock != null)
                            {
                                if (!stockAdjustments.ContainsKey(stock.Id))
                                    stockAdjustments[stock.Id] = 0;

                                stockAdjustments[stock.Id] -= comboItem.Qty * item.Qty;
                            }
                        }
                    }
                }
            }

            foreach (var adjustment in stockAdjustments)
            {
                var stock = await unitOfWork.StockRepository.GetByIdAsync(adjustment.Key);

                if (stock != null)
                {
                    stock.Qty += adjustment.Value;
                    unitOfWork.StockRepository.Update(stock);
                }
            }
        }

        private async Task AddPettyCashMovement(IUnitOfWork unitOfWork, PettyCash pettyCash, Invoice invoice)
        {
            var movement = new PettyCashMovement
            {
                PettyCashId = pettyCash.Id,
                PettyCash = pettyCash,
                InvoiceId = invoice.Id,
                Invoice = invoice,
                Amount = invoice.TotalPrice,
                Type = MovementType.Income,
            };

            await unitOfWork.PettyCashMovementRepository.CreateAsync(movement);

            var cashPaymentsTotal = Dto.Payments
                .Where(x => x.PaymentId == PaymentMethod.Cash)
                .Sum(x => x.Amount);

            pettyCash.ClosingAmount += cashPaymentsTotal;

            unitOfWork.PettyCashRepository.Update(pettyCash);
        }

        private static async Task<(double basePrice, double taxAmount)> GetItemPricingAndTax(
            SaleInvoiceItemDTO itemDTO,
            IUnitOfWork unitOfWork,
            double currencyRate)
        {
            var item = await unitOfWork.ItemRepository.GetByIdAsync(itemDTO.ItemId);
            if (item == null) return (0, 0);

            var pricingItem = item.PricingItems?.FirstOrDefault(x => x.ItemId == itemDTO.ItemId);
            var basePrice = (pricingItem?.BasePrice ?? 0) / currencyRate;
            var taxAmount = item.ItemTaxes?.Sum(x => x?.Tax?.Amount) ?? 0;

            return (basePrice, taxAmount);
        }
    }
}
