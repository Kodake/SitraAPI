using AutoMapper;
using BackEnd.DTOs.Invoices.SaleInvoices;
using BackEnd.Entities.Accounts;
using BackEnd.Entities.AdministrativeTools;
using BackEnd.Entities.Invoices;
using BackEnd.Enums;
using BackEnd.Repositories;
using BackEnd.Validations;
using FluentValidation;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.MediatR.Commands.Invoices.Quotations
{
    public class ProcessQuotationCommand : UpdateCommand<Quotation, CreateSaleInvoiceDTO>
    {
        public ProcessQuotationCommand(long id, CreateSaleInvoiceDTO dto) : base(id, dto) { }

        public override async Task<bool> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper, CancellationToken cancellationToken)
        {
            // Validate DTO
            var validator = new CreateSaleInvoiceDTOValidator(unitOfWork);
            var validationResult = await validator.ValidateAsync(Dto);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var quotation = await unitOfWork.QuotationRepository.GetByIdAsync(Id);
            if (quotation == null)
            {
                return false;
            }

            var fiscalSupportDB = await unitOfWork.FiscalSupportRepository.GetByIdAsync(Dto.FiscalSupportId);
            var currencyDB = await unitOfWork.CurrencyRepository.GetByIdAsync(Dto.CurrencyId);

            if (fiscalSupportDB == null || currencyDB == null)
            {
                throw new Exception("Fiscal support or currency not found.");
            }

            var saleInvoice = mapper.Map<Invoice>(Dto);
            saleInvoice.InvoiceType = InvoiceType.Sale;
            saleInvoice.FiscalSupportNumber = GenerateFiscalSupportNumber(fiscalSupportDB);
            unitOfWork.FiscalSupportRepository.Update(fiscalSupportDB);

            double currencyRate = currencyDB.Rate;
            await CalculateInvoiceTotals(Dto.InvoiceItems, saleInvoice, unitOfWork, currencyRate);

            await unitOfWork.SaleInvoiceRepository.CreateAsync(saleInvoice);

            var payments = Dto.Payments.Select(p => mapper.Map<Payment>(p)).ToList();

            switch (Dto.ConditionType)
            {
                case InvoiceCondition.Credit:
                    var accountsReceivable = new Receivable
                    {
                        Invoice = saleInvoice,
                        ContactId = saleInvoice.CustomerId,
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
                        ContactId = saleInvoice.CustomerId,
                        AmountReturned = Dto.AmountReturned ?? 0,
                        PaymentDate = DateTime.UtcNow,
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

            await AddInvoiceItems(Dto.InvoiceItems, saleInvoice, unitOfWork, mapper, currencyRate);
            await ReduceStock(Dto, unitOfWork);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("fiscal-supports-get", cancellationToken);
            await outputCacheStore.EvictByTagAsync("stocks-get", cancellationToken);
            await outputCacheStore.EvictByTagAsync("receivables-get", cancellationToken);
            await outputCacheStore.EvictByTagAsync("spotPayments-get", cancellationToken);

            return true;
        }

        private static string GenerateFiscalSupportNumber(FiscalSupport fiscalSupport)
        {
            string fiscalSupportName = Enum.GetName(typeof(FiscalSupportType), fiscalSupport.FiscalSupportType) ?? "Unknown";
            char symbolChar = fiscalSupport.Symbol.Length > 0 ? fiscalSupport.Symbol[0] : '?';
            int validSpaceNumber = (int)fiscalSupport.SpaceNumber;
            string repeatedSymbols = new string(symbolChar, validSpaceNumber);
            string sequence = ((int)fiscalSupport.InitialSequence).ToString().PadLeft(validSpaceNumber, symbolChar);
            string finalSymbols = repeatedSymbols[..^validSpaceNumber] + sequence;

            return $"{fiscalSupportName}{finalSymbols}";
        }

        private static async Task ReduceStock(CreateSaleInvoiceDTO dto, IUnitOfWork unitOfWork)
        {
            foreach (var item in dto.InvoiceItems)
            {
                if (item.ItemType == ItemType.Product)
                {
                    var stock = await unitOfWork.StockRepository.GetByItemIdAndWarehouseId(item.ItemId, dto.WarehouseId) ?? throw new InvalidOperationException($"Stock not found for ItemId {item.ItemId} in the specified warehouse.");
                    stock.Qty -= item.Qty;
                    unitOfWork.StockRepository.Update(stock);
                }
            }
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
                        var product = await unitOfWork.ProductRepository.GetByIdAsync(itemDTO.ItemId)
                            ?? throw new Exception($"Product with id {itemDTO.ItemId} not found.");

                        var productBasePrice = product.PricingItems.First(x => x.ItemId == itemDTO.ItemId).BasePrice / currencyRate;
                        var productTaxAmount = product.ItemTaxes.Sum(x => x?.Tax?.Amount) ?? 0;

                        totalQtyItems += itemDTO.Qty;
                        totalItems += 1;
                        totalAmount += productBasePrice * itemDTO.Qty;
                        totalTaxes += productBasePrice * productTaxAmount / 100 * itemDTO.Qty;
                        break;

                    case ItemType.Service:
                        var service = await unitOfWork.ServiceRepository.GetByIdAsync(itemDTO.ItemId)
                            ?? throw new Exception($"Service with id {itemDTO.ItemId} not found.");

                        var serviceBasePrice = service.PricingItems.First(x => x.ItemId == itemDTO.ItemId).BasePrice / currencyRate;
                        var serviceTaxAmount = service.ItemTaxes.Sum(x => x?.Tax?.Amount) ?? 0;

                        totalQtyItems += itemDTO.Qty;
                        totalItems += 1;
                        totalAmount += serviceBasePrice * itemDTO.Qty;
                        totalTaxes += serviceBasePrice * serviceTaxAmount / 100 * itemDTO.Qty;
                        break;

                    default:
                        throw new InvalidOperationException($"Unknown ItemType {itemDTO.ItemType} for ItemId {itemDTO.ItemId}");
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
            var item = await unitOfWork.ItemRepository.GetByIdAsync(itemDTO.ItemId)
                ?? throw new Exception($"Item with id {itemDTO.ItemId} not found.");

            var basePrice = item.PricingItems.First(x => x.ItemId == itemDTO.ItemId).BasePrice / currencyRate;
            var taxAmount = item.ItemTaxes.Sum(x => x?.Tax?.Amount) ?? 0;

            return (basePrice, taxAmount);
        }
    }
}
