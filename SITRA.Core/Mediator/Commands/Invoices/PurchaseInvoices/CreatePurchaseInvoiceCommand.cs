using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.DTOs.Invoices.PurchaseInvoices;
using SITRA.Core.Entities.Accounts;
using SITRA.Core.Entities.CashBoxes;
using SITRA.Core.Entities.Invoices;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Commands.Invoices.PurchaseInvoices
{
    public class CreatePurchaseInvoiceCommand(CreatePurchaseInvoiceDTO dto) : CreateCommand<Invoice, CreatePurchaseInvoiceDTO>(dto), IIdempotentCommand
    {
        public Func<Invoice, Task<(bool isAccepted, string trackId, string status, string securityCode, string rawResponse)>>? DgiiProcess { get; set; }

        public override async Task<CreatePurchaseInvoiceDTO> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper, CancellationToken cancellationToken)
        {
            // All validations are handled by FluentValidation before this action is executed
            var user = await unitOfWork.UserService.GetUserAsync();
            var pettyCash = await unitOfWork.PettyCashRepository.GetOpenPettyCashAsync();

            var fiscalSupportDB = Dto.FiscalSupportId.HasValue
                ? await unitOfWork.FiscalSupportRepository.GetByIdAsync(Dto.FiscalSupportId.Value)
                : null;

            var currencyDB = await unitOfWork.CurrencyRepository.GetByIdAsync(Dto.CurrencyId);

            var purchaseInvoice = mapper.Map<Invoice>(Dto);
            purchaseInvoice.InvoiceType = InvoiceType.Purchase;
            purchaseInvoice.Date = DateTimeOffset.Now;
            if (Dto.SupplierId != Guid.Empty)
            {
                purchaseInvoice.InvoiceBuyer = new InvoiceBuyer { ContactId = Dto.SupplierId };
            }

            if (fiscalSupportDB != null)
            {
                fiscalSupportDB.IncrementSequence();
                purchaseInvoice.FiscalSupportNumber = fiscalSupportDB.GenerateFiscalSupportNumber();
                purchaseInvoice.InvoiceFiscalSupport = new InvoiceFiscalSupport { FiscalSupportId = fiscalSupportDB.Id };
                unitOfWork.FiscalSupportRepository.Update(fiscalSupportDB);
            }

            double currencyRate = currencyDB!.Rate;
            await CalculateInvoiceTotals(Dto.InvoiceItems, purchaseInvoice, unitOfWork, currencyRate);

            await unitOfWork.PurchaseInvoiceRepository.CreateAsync(purchaseInvoice);
            await AddInvoiceItems(Dto.InvoiceItems, purchaseInvoice, unitOfWork, mapper, currencyRate);

            // First save to generate IDs and establish relationships
            await unitOfWork.SaveChangesAsync();

            // Perform DGII Workflow
            if (DgiiProcess != null && !string.IsNullOrEmpty(purchaseInvoice.FiscalSupportNumber))
            {
                var fullInvoice = await unitOfWork.PurchaseInvoiceRepository.GetByIdAsync(purchaseInvoice.Id, cancellationToken);
                var dgiiResult = await DgiiProcess(fullInvoice!);

                purchaseInvoice.DgiiTrackId = string.IsNullOrEmpty(dgiiResult.trackId) ? null : dgiiResult.trackId;
                purchaseInvoice.DgiiStatus = dgiiResult.status;
                purchaseInvoice.DgiiResponse = dgiiResult.rawResponse;
                purchaseInvoice.SecurityCode = dgiiResult.securityCode;

                unitOfWork.PurchaseInvoiceRepository.Update(purchaseInvoice);
                await unitOfWork.SaveChangesAsync();

                if (!dgiiResult.isAccepted)
                {
                    // If rejected by DGII, we skip stock reduction, payments, etc., but keep the invoice record.
                    Dto.Id = purchaseInvoice.Id;
                    return Dto;
                }
            }

            switch (Dto.ConditionType)
            {
                case InvoiceCondition.Credit:
                    var accountsPayable = new Payable
                    {
                        Invoice = purchaseInvoice,
                        ContactId = Dto.SupplierId != Guid.Empty ? Dto.SupplierId : null,
                        TotalAmountDue = purchaseInvoice.TotalPrice,
                        TotalOutstanding = 0,
                        Status = AccountStatus.PendingPayable,
                    };

                    await unitOfWork.PayableRepository.CreateAsync(accountsPayable);
                    break;

                case InvoiceCondition.Spot:
                    var spotPayable = new SpotPayable
                    {
                        Invoice = purchaseInvoice,
                        ContactId = Dto.SupplierId != Guid.Empty ? Dto.SupplierId : null,
                        PaymentMethod = Dto.PaymentMethod,
                        PaymentDate = DateTimeOffset.UtcNow,
                        IsActive = true
                    };

                    await unitOfWork.SpotPayableRepository.CreateAsync(spotPayable);
                    break;
            }

            await IncreaseStock(Dto, unitOfWork);
            await AddPettyCashMovement(unitOfWork, pettyCash!, purchaseInvoice);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("fiscal-supports-get", cancellationToken);
            await outputCacheStore.EvictByTagAsync("stocks-get", cancellationToken);
            await outputCacheStore.EvictByTagAsync("payables-get", cancellationToken);
            await outputCacheStore.EvictByTagAsync("spotPayables-get", cancellationToken);
            await outputCacheStore.EvictByTagAsync("pettyCashMovements-get", cancellationToken);
            Dto.Id = purchaseInvoice.Id;

            return Dto;
        }

        private static async Task CalculateInvoiceTotals(
            IEnumerable<PurchaseInvoiceItemDTO> items,
            Invoice invoice,
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
                        totalTaxes += (productBasePrice * productTaxAmount / 100) * itemDTO.Qty;
                        break;

                    case ItemType.Service:
                        var service = await unitOfWork.ServiceRepository.GetByIdAsync(itemDTO.ItemId);

                        var serviceBasePrice = service!.PricingItems.First(x => x.ItemId == itemDTO.ItemId).BasePrice / currencyRate;
                        var serviceTaxAmount = service.ItemTaxes.Sum(x => x?.Tax?.Amount) ?? 0;

                        totalQtyItems += itemDTO.Qty;
                        totalItems += 1;
                        totalAmount += serviceBasePrice * itemDTO.Qty;
                        totalTaxes += (serviceBasePrice * serviceTaxAmount / 100) * itemDTO.Qty;
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
            invoice.TotalQtyItems = totalQtyItems;
            invoice.TotalItems = totalItems;
            invoice.TotalPrice = invoice.Discount.HasValue && invoice.Discount > 0
                ? (double)((totalPrice + (invoice.Shipping ?? 0))
                   - ((totalPrice + (invoice.Shipping ?? 0)) * invoice.Discount.Value / 100))
                : totalPrice + (invoice.Shipping ?? 0);
            invoice.TotalAmount = totalAmount;
            invoice.TotalTaxes = totalTaxes;
        }

        private static async Task AddInvoiceItems(
            IEnumerable<PurchaseInvoiceItemDTO> items,
            Invoice invoice,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            double currencyRate)
        {
            foreach (var itemDTO in items)
            {
                var purchaseInvoiceItem = invoice.InvoiceItems.FirstOrDefault(i => i.ItemId == itemDTO.ItemId);
                if (purchaseInvoiceItem == null)
                {
                    purchaseInvoiceItem = mapper.Map<InvoiceItem>(itemDTO);
                    invoice.InvoiceItems.Add(purchaseInvoiceItem);
                }

                purchaseInvoiceItem.InvoiceId = invoice.Id;
                purchaseInvoiceItem.Invoice = invoice;

                var (basePrice, taxAmount) = await GetItemPricingAndTax(itemDTO, unitOfWork, currencyRate);

                purchaseInvoiceItem.TotalAmount = basePrice * itemDTO.Qty;
                purchaseInvoiceItem.TotalTaxes = basePrice * taxAmount / 100 * itemDTO.Qty;
                purchaseInvoiceItem.TotalPrice = purchaseInvoiceItem.TotalAmount + purchaseInvoiceItem.TotalTaxes;
                purchaseInvoiceItem.BasePrice = basePrice;

                await unitOfWork.SaleInvoiceRepository.AddInvoiceItem(purchaseInvoiceItem);
            }
        }

        private static async Task IncreaseStock(CreatePurchaseInvoiceDTO dto, IUnitOfWork unitOfWork)
        {
            foreach (var item in dto.InvoiceItems)
            {
                if (item.ItemType == ItemType.Product)
                {
                    var stock = await unitOfWork.StockRepository.GetByItemIdAndWarehouseId(item.ItemId, dto.WarehouseId);
                    stock!.Qty += item.Qty;
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
                Type = MovementType.Expense,
            };

            await unitOfWork.PettyCashMovementRepository.CreateAsync(movement);

            if (Dto.ConditionType == InvoiceCondition.Spot && Dto.PaymentMethod == PaymentMethod.Cash)
            {
                pettyCash.ClosingAmount -= invoice.TotalPrice;
                unitOfWork.PettyCashRepository.Update(pettyCash);
            }
        }

        private static async Task<(double basePrice, double taxAmount)> GetItemPricingAndTax(
            PurchaseInvoiceItemDTO itemDTO,
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
