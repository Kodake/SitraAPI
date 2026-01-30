using AutoMapper;
using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.DTOs.Invoices.SaleInvoices;
using SITRA.Core.Entities.Invoices;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Commands.Invoices.Quotations
{
    public class CreateQuotationCommand : CreateCommand<Quotation, CreateQuotationDTO>, IIdempotentCommand
    {
        public CreateQuotationCommand(CreateQuotationDTO dto) : base(dto) { }

        public override async Task<CreateQuotationDTO> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper, CancellationToken cancellationToken)
        {
            // All validations are handled by FluentValidation before this action is executed
            var currencyDB = await unitOfWork.CurrencyRepository.GetByIdAsync(Dto.CurrencyId);

            var saleInvoice = mapper.Map<Invoice>(Dto);
            saleInvoice.Date = DateTimeOffset.Now;
            saleInvoice.InvoiceType = InvoiceType.Quotation;
            if (Dto.CustomerId != Guid.Empty)
            {
                saleInvoice.InvoiceBuyer = new InvoiceBuyer { ContactId = Dto.CustomerId };
            }

            if (Dto.BillerId != Guid.Empty)
            {
                saleInvoice.InvoiceBiller = new InvoiceBiller { ContactId = Dto.BillerId };
            }

            double currencyRate = currencyDB!.Rate;
            await CalculateInvoiceTotals(Dto.InvoiceItems, saleInvoice, unitOfWork, currencyRate);

            await unitOfWork.SaleInvoiceRepository.CreateAsync(saleInvoice);

            var quotation = new Quotation
            {
                Invoice = saleInvoice,
                ContactId = Dto.CustomerId != Guid.Empty ? Dto.CustomerId : null,
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
