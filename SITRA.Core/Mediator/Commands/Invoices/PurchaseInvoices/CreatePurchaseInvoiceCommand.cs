using AutoMapper;
using SITRA.Core.DTOs.Invoices.PurchaseInvoices;
using SITRA.Core.Entities.Accounts;
using SITRA.Core.Entities.Invoices;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;
using SITRA.Core.Validations;
using FluentValidation;
using Microsoft.AspNetCore.OutputCaching;

namespace SITRA.Core.Mediator.Commands.Invoices.PurchaseInvoices
{
    public class CreatePurchaseInvoiceCommand : CreateCommand<Invoice, CreatePurchaseInvoiceDTO>
    {
        public CreatePurchaseInvoiceCommand(CreatePurchaseInvoiceDTO dto) : base(dto) { }

        public override async Task<CreatePurchaseInvoiceDTO> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper, CancellationToken cancellationToken)
        {
            // All validations are handled by FluentValidation before this action is executed
            var currencyDB = await unitOfWork.CurrencyRepository.GetByIdAsync(Dto.CurrencyId);

            var purchaseInvoice = mapper.Map<Invoice>(Dto);
            purchaseInvoice.InvoiceType = InvoiceType.Purchase;
            purchaseInvoice.Date = DateTimeOffset.UtcNow;
            if (Dto.SupplierId > 0)
            {
                purchaseInvoice.InvoiceBuyer = new InvoiceBuyer { ContactId = Dto.SupplierId };
            }

            double currencyRate = currencyDB!.Rate;
            await CalculateInvoiceTotals(Dto.InvoiceItems, purchaseInvoice, unitOfWork, currencyRate);

            await unitOfWork.PurchaseInvoiceRepository.CreateAsync(purchaseInvoice);

            switch (Dto.ConditionType)
            {
                case InvoiceCondition.Credit:
                    var accountsPayable = new Payable
                    {
                        Invoice = purchaseInvoice,
                        ContactId = Dto.SupplierId != 0 ? Dto.SupplierId : null,
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
                        ContactId = Dto.SupplierId != 0 ? Dto.SupplierId : null,
                        PaymentMethod = Dto.PaymentMethod,
                        PaymentDate = DateTimeOffset.UtcNow,
                        IsActive = true
                    };

                    await unitOfWork.SpotPayableRepository.CreateAsync(spotPayable);
                    break;
            }

            await AddInvoiceItems(Dto.InvoiceItems, purchaseInvoice, unitOfWork, mapper, currencyRate);
            await IncreaseStock(Dto, unitOfWork);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("stocks-get", cancellationToken);
            await outputCacheStore.EvictByTagAsync("payables-get", cancellationToken);
            await outputCacheStore.EvictByTagAsync("spotPayables-get", cancellationToken);

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
