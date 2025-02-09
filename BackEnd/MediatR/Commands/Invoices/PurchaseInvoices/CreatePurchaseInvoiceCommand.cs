using AutoMapper;
using BackEnd.DTOs.Invoices.PurchaseInvoices;
using BackEnd.DTOs.Invoices.SaleInvoices;
using BackEnd.Entities.Accounts;
using BackEnd.Entities.AdministrativeTools;
using BackEnd.Entities.Invoices;
using BackEnd.Enums;
using BackEnd.Repositories;
using BackEnd.Validations;
using FluentValidation;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.MediatR.Commands.Invoices.PurchaseInvoices
{
    public class CreatePurchaseInvoiceCommand : CreateCommand<Invoice, CreatePurchaseInvoiceDTO>
    {
        public CreatePurchaseInvoiceCommand(CreatePurchaseInvoiceDTO dto) : base(dto) { }

        public override async Task<CreatePurchaseInvoiceDTO> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper, CancellationToken cancellationToken)
        {
            // Validate DTO
            var validator = new CreatePurchaseInvoiceDTOValidator(unitOfWork);
            var validationResult = await validator.ValidateAsync(Dto);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var currencyDB = await unitOfWork.CurrencyRepository.GetByIdAsync(Dto.CurrencyId);

            if (currencyDB == null)
            {
                throw new Exception("Currency not found.");
            }

            var purchaseInvoice = mapper.Map<Invoice>(Dto);
            purchaseInvoice.InvoiceType = InvoiceType.Purchase;

            double currencyRate = currencyDB.Rate;
            await CalculateInvoiceTotals(Dto.InvoiceItems, purchaseInvoice, unitOfWork, currencyRate);

            await unitOfWork.PurchaseInvoiceRepository.CreateAsync(purchaseInvoice);

            switch (Dto.ConditionType)
            {
                case InvoiceCondition.Credit:
                    var accountsPayable = new Payable
                    {
                        Invoice = purchaseInvoice,
                        ContactId = purchaseInvoice.CustomerId,
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
                        ContactId = purchaseInvoice.CustomerId,
                        PaymentMethod = Dto.PaymentMethod,
                        PaymentDate = DateTime.UtcNow,
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
                        var product = await unitOfWork.ProductRepository.GetByIdAsync(itemDTO.ItemId)
                            ?? throw new Exception($"Product with id {itemDTO.ItemId} not found.");

                        var productBasePrice = product.PricingItems.First(x => x.ItemId == itemDTO.ItemId).BasePrice / currencyRate;
                        var productTaxAmount = product.ItemTaxes.Sum(x => x?.Tax?.Amount) ?? 0;

                        totalQtyItems += itemDTO.Qty;
                        totalItems += 1;
                        totalAmount += productBasePrice * itemDTO.Qty;
                        totalTaxes += (productBasePrice * productTaxAmount / 100) * itemDTO.Qty;
                        break;

                    case ItemType.Service:
                        var service = await unitOfWork.ServiceRepository.GetByIdAsync(itemDTO.ItemId)
                            ?? throw new Exception($"Service with id {itemDTO.ItemId} not found.");

                        var serviceBasePrice = service.PricingItems.First(x => x.ItemId == itemDTO.ItemId).BasePrice / currencyRate;
                        var serviceTaxAmount = service.ItemTaxes.Sum(x => x?.Tax?.Amount) ?? 0;

                        totalQtyItems += itemDTO.Qty;
                        totalItems += 1;
                        totalAmount += serviceBasePrice * itemDTO.Qty;
                        totalTaxes += (serviceBasePrice * serviceTaxAmount / 100) * itemDTO.Qty;
                        break;

                    default:
                        throw new InvalidOperationException($"Unknown ItemType {itemDTO.ItemType} for ItemId {itemDTO.ItemId}");
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
                    var stock = await unitOfWork.StockRepository.GetByItemIdAndWarehouseId(item.ItemId, dto.WarehouseId) ?? throw new InvalidOperationException($"Stock not found for ItemId {item.ItemId} in the specified warehouse.");
                    stock.Qty += item.Qty;
                    unitOfWork.StockRepository.Update(stock);
                }
            }
        }

        private static async Task<(double basePrice, double taxAmount)> GetItemPricingAndTax(
            PurchaseInvoiceItemDTO itemDTO,
            IUnitOfWork unitOfWork,
            double currencyRate)
        {
            var item = await unitOfWork.ItemRepository.GetByIdAsync(itemDTO.ItemId)
                ?? throw new Exception($"Item with id {itemDTO.ItemId} not found.");

            var basePrice = item.PricingItems.First(x => x.ItemId == itemDTO.ItemId).BasePrice / currencyRate;
            var taxAmount = item.ItemTaxes.Sum(x => x?.Tax?.Amount) ?? 0;

            return (basePrice, taxAmount);
        }
    }
}
