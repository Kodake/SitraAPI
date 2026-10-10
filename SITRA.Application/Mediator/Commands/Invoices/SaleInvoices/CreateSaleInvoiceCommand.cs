using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.DTOs.Invoices.SaleInvoices;
using SITRA.Core.Entities.Accounts;
using SITRA.Core.Entities.CashBoxes;
using SITRA.Core.Entities.Invoices;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Commands.Invoices.SaleInvoices
{
    public class CreateSaleInvoiceCommand(CreateSaleInvoiceDTO dto, string? modifiedNcfNumber = null) : CreateCommand<Invoice, CreateSaleInvoiceDTO>(dto), IIdempotentCommand
    {
        public string? ModifiedNcfNumber { get; } = modifiedNcfNumber;

        public Func<Invoice, Task<(bool isAccepted, string trackId, string status, string securityCode, string rawResponse)>>? DgiiProcess { get; set; }

        public override async Task<CreateSaleInvoiceDTO> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper, CancellationToken cancellationToken)
        {
            // All validations are handled by FluentValidation before this action is executed
            var user = await unitOfWork.UserService.GetUserAsync();
            var pettyCash = await unitOfWork.PettyCashRepository.GetOpenPettyCashAsync();

            var fiscalSupportDB = Dto.FiscalSupportId.HasValue && Dto.FiscalSupportId.Value != Guid.Empty
                ? await unitOfWork.FiscalSupportRepository.GetByIdAsync(Dto.FiscalSupportId.Value)
                : null;

            if (fiscalSupportDB == null && !string.IsNullOrEmpty(Dto.FiscalSupportType) && int.TryParse(Dto.FiscalSupportType, out var parsedFiscalType))
            {
                var supports = await unitOfWork.FiscalSupportRepository.GetAllAsync(fs => (int)fs.FiscalSupportType == parsedFiscalType && fs.InitialSequence < fs.FinalSequence);
                fiscalSupportDB = supports.FirstOrDefault();
            }

            var currencyDB = await unitOfWork.CurrencyRepository.GetByIdAsync(Dto.CurrencyId);

            var saleInvoice = mapper.Map<Invoice>(Dto);
            saleInvoice.InvoiceType = InvoiceType.Sale;
            saleInvoice.Date = DateTimeOffset.Now;
            saleInvoice.ModifiedNcfNumber = ModifiedNcfNumber;
            if (!string.IsNullOrEmpty(ModifiedNcfNumber))
                saleInvoice.ModificationCode = 3; // Default: 1 = Anula el NCF modificado

            if (saleInvoice.Id == Guid.Empty)
            {
                saleInvoice.Id = Guid.NewGuid();
            }

            saleInvoice.BuyerId = (Dto.CustomerId.HasValue && Dto.CustomerId.Value != Guid.Empty)
                ? Dto.CustomerId.Value
                : null;

            saleInvoice.BillerId = (Dto.BillerId.HasValue && Dto.BillerId.Value != Guid.Empty)
                ? Dto.BillerId.Value
                : null;

            if (fiscalSupportDB != null)
            {
                fiscalSupportDB.IncrementSequence();
                saleInvoice.FiscalSupportNumber = fiscalSupportDB.GenerateFiscalSupportNumber();
                saleInvoice.FiscalSupportId = fiscalSupportDB.Id;
                unitOfWork.FiscalSupportRepository.Update(fiscalSupportDB);
            }
            else
            {
                saleInvoice.FiscalSupportId = (Dto.FiscalSupportId.HasValue && Dto.FiscalSupportId.Value != Guid.Empty)
                    ? Dto.FiscalSupportId.Value
                    : null;
            }

            double currencyRate = currencyDB!.Rate;
            await CalculateInvoiceTotals(Dto.InvoiceItems, saleInvoice, unitOfWork, currencyRate);
            await AddInvoiceItems(Dto.InvoiceItems, saleInvoice, unitOfWork, mapper, currencyRate);

            await unitOfWork.SaleInvoiceRepository.CreateAsync(saleInvoice);

            // First save to generate IDs and establish relationships
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
                    // If rejected by DGII, we skip stock reduction, payments, etc., but keep the invoice record.
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
                        Id = Guid.NewGuid(),
                        Invoice = saleInvoice,
                        InvoiceId = saleInvoice.Id,
                        ContactId = Dto.CustomerId.HasValue && Dto.CustomerId.Value != Guid.Empty ? Dto.CustomerId.Value : null,
                        TotalAmountDue = saleInvoice.TotalPrice,
                        TotalOutstanding = 0,
                        Status = AccountStatus.PendingReceivable,
                    };

                    await unitOfWork.ReceivableRepository.CreateAsync(accountsReceivable);
                    break;

                case InvoiceCondition.Spot:
                    var spotPayment = new SpotPayment
                    {
                        Id = Guid.NewGuid(),
                        Invoice = saleInvoice,
                        InvoiceId = saleInvoice.Id,
                        ContactId = Dto.CustomerId.HasValue && Dto.CustomerId.Value != Guid.Empty ? Dto.CustomerId.Value : null,
                        AmountReturned = Dto.AmountReturned ?? 0,
                        PaymentDate = DateTimeOffset.UtcNow,
                        IsActive = true
                    };

                    await unitOfWork.SpotPaymentRepository.CreateAsync(spotPayment);

                    foreach (var payment in payments)
                    {
                        if (payment.Id == Guid.Empty) payment.Id = Guid.NewGuid();
                        payment.SpotPayment = spotPayment;
                        payment.SpotPaymentId = spotPayment.Id;
                        await unitOfWork.PaymentRepository.CreateAsync(payment);
                    }
                    break;
            }

            await ReduceStock(Dto, unitOfWork);
            await AddPettyCashMovement(unitOfWork, pettyCash, saleInvoice);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("fiscal-supports-get", cancellationToken);
            await outputCacheStore.EvictByTagAsync("stocks-get", cancellationToken);
            await outputCacheStore.EvictByTagAsync("receivables-get", cancellationToken);
            await outputCacheStore.EvictByTagAsync("spotPayments-get", cancellationToken);
            await outputCacheStore.EvictByTagAsync("pettyCashMovements-get", cancellationToken);
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
                        var prodPricing = (saleInvoice.PricingId != Guid.Empty ? product?.PricingItems.FirstOrDefault(x => x.PricingId == saleInvoice.PricingId) : null)
                                          ?? product?.PricingItems.FirstOrDefault(x => x.ItemId == itemDTO.ItemId)
                                          ?? product?.PricingItems.FirstOrDefault();

                        var productBasePrice = (prodPricing?.BasePrice ?? 0) / currencyRate;
                        var productTaxAmount = product?.ItemTaxes.Sum(x => x?.Tax?.Amount) ?? 0;

                        totalQtyItems += itemDTO.Qty;
                        totalItems += 1;
                        totalAmount += productBasePrice * itemDTO.Qty;
                        totalTaxes += (productBasePrice * productTaxAmount / 100) * itemDTO.Qty;
                        break;

                    case ItemType.Service:
                        var service = await unitOfWork.ServiceRepository.GetByIdAsync(itemDTO.ItemId);
                        var servPricing = (saleInvoice.PricingId != Guid.Empty ? service?.PricingItems.FirstOrDefault(x => x.PricingId == saleInvoice.PricingId) : null)
                                          ?? service?.PricingItems.FirstOrDefault(x => x.ItemId == itemDTO.ItemId)
                                          ?? service?.PricingItems.FirstOrDefault();

                        var serviceBasePrice = (servPricing?.BasePrice ?? 0) / currencyRate;
                        var serviceTaxAmount = service?.ItemTaxes.Sum(x => x?.Tax?.Amount) ?? 0;

                        totalQtyItems += itemDTO.Qty;
                        totalItems += 1;
                        totalAmount += serviceBasePrice * itemDTO.Qty;
                        totalTaxes += (serviceBasePrice * serviceTaxAmount / 100) * itemDTO.Qty;
                        break;

                    case ItemType.Combo:
                        var combo = await unitOfWork.ComboRepository.GetByIdAsync(itemDTO.ItemId);

                        var comboPricing = combo?.PricingItems.FirstOrDefault(x => x.PricingId == saleInvoice.PricingId)
                                           ?? combo?.PricingItems.FirstOrDefault(x => x.ItemId == itemDTO.ItemId)
                                           ?? combo?.PricingItems.FirstOrDefault();
                        var comboBasePrice = (comboPricing?.BasePrice ?? 0) / currencyRate;
                        var comboTaxAmount = combo?.ItemTaxes.Sum(x => x?.Tax?.Amount) ?? 0;

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
            saleInvoice.InvoiceItems.Clear();

            foreach (var itemDTO in items)
            {
                var saleInvoiceItem = mapper.Map<InvoiceItem>(itemDTO);
                saleInvoiceItem.Id = Guid.NewGuid();
                saleInvoiceItem.InvoiceId = saleInvoice.Id;
                saleInvoiceItem.Invoice = saleInvoice;

                var (basePrice, taxAmount) = await GetItemPricingAndTax(itemDTO, unitOfWork, currencyRate, saleInvoice.PricingId);

                saleInvoiceItem.TotalAmount = basePrice * itemDTO.Qty;
                saleInvoiceItem.TotalTaxes = basePrice * taxAmount / 100 * itemDTO.Qty;
                saleInvoiceItem.TotalPrice = saleInvoiceItem.TotalAmount + saleInvoiceItem.TotalTaxes;
                saleInvoiceItem.BasePrice = basePrice;

                saleInvoice.InvoiceItems.Add(saleInvoiceItem);
            }
        }

        private static async Task ReduceStock(CreateSaleInvoiceDTO dto, IUnitOfWork unitOfWork)
        {
            // Diccionario para acumular restas de stock
            var stockAdjustments = new Dictionary<Guid, double>(); // StockId -> cantidad a restar

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

            // ?? aplicar todos los cambios a los stocks en una sola pasada
            foreach (var adjustment in stockAdjustments)
            {
                var stock = await unitOfWork.StockRepository.GetByIdAsync(adjustment.Key);

                stock!.Qty += adjustment.Value; // Value es negativo ? resta
                unitOfWork.StockRepository.Update(stock);
            }
        }

        private async Task AddPettyCashMovement(IUnitOfWork unitOfWork, PettyCash? pettyCash, Invoice invoice)
        {
            if (pettyCash == null) return;

            var movement = new PettyCashMovement
            {
                Id = Guid.NewGuid(),
                PettyCashId = pettyCash.Id,
                PettyCash = pettyCash,
                InvoiceId = invoice.Id,
                Invoice = invoice,
                Amount = invoice.TotalPrice,
                Type = MovementType.Income,
                CreatedAt = DateTimeOffset.UtcNow
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
            double currencyRate,
            Guid? pricingId = null)
        {
            var item = await unitOfWork.ItemRepository.GetByIdAsync(itemDTO.ItemId);

            var pricing = (pricingId.HasValue && pricingId.Value != Guid.Empty ? item?.PricingItems.FirstOrDefault(x => x.PricingId == pricingId.Value) : null)
                          ?? item?.PricingItems.FirstOrDefault(x => x.ItemId == itemDTO.ItemId)
                          ?? item?.PricingItems.FirstOrDefault();

            var basePrice = (pricing?.BasePrice ?? 0) / currencyRate;
            var taxAmount = item?.ItemTaxes.Sum(x => x?.Tax?.Amount) ?? 0;

            return (basePrice, taxAmount);
        }
    }
}

