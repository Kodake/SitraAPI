using AutoMapper;
using BackEnd.DTOs.Invoices.SaleInvoices;
using BackEnd.Entities.Invoices;
using BackEnd.Enums;
using BackEnd.Repositories;
using BackEnd.Repositories.Invoices;
using static BackEnd.Helpers.Utilities;

namespace BackEnd.Services.Invoices
{
    public class SaleInvoiceService : ISaleInvoiceService
    {
        public async Task CalculateInvoiceTotalsAsync(IEnumerable<SaleInvoiceItemDTO> items, SaleInvoice saleInvoice, IUnitOfWork unitOfWork, double currencyRate)
        {
            double totalQtyItems = 0, totalItems = 0, totalAmount = 0, totalTaxes = 0;

            foreach (var itemDTO in items)
            {
                var product = await unitOfWork.ProductRepository.GetByIdAsync(itemDTO.ItemId)
                               ?? throw new Exception($"Product with id {itemDTO.ItemId} not found.");

                var basePrice = product.PricingItems.First(x => x.ItemId == itemDTO.ItemId).BasePrice / currencyRate;
                var taxAmount = product.ItemTaxes.First(x => x.ItemId == itemDTO.ItemId).Tax.Amount / currencyRate;

                totalQtyItems += itemDTO.Qty;
                totalItems += 1;
                totalAmount += InvoiceItemCalculator.CalculateTotalAmount(basePrice, itemDTO.Qty);
                totalTaxes += InvoiceItemCalculator.CalculateTotalTaxes(basePrice, taxAmount, itemDTO.Qty);
            }

            double totalPrice = InvoiceItemCalculator.CalculateTotalPrice(totalAmount, totalTaxes);

            saleInvoice.TotalQtyItems = totalQtyItems;
            saleInvoice.TotalItems = totalItems;
            saleInvoice.TotalPrice = totalPrice + (saleInvoice.Shipping ?? 0);
            saleInvoice.TotalAmount = totalAmount;
            saleInvoice.TotalTaxes = totalTaxes;
        }

        public async Task AddInvoiceItemsAsync(IEnumerable<SaleInvoiceItemDTO> items, long invoiceId, ISaleInvoiceRepository repository, IUnitOfWork unitOfWork, IMapper mapper, double currencyRate)
        {
            foreach (var itemDTO in items)
            {
                var product = await unitOfWork.ProductRepository.GetByIdAsync(itemDTO.ItemId)
                               ?? throw new Exception($"Product with id {itemDTO.ItemId} not found.");

                var saleInvoiceItem = mapper.Map<SaleInvoiceItem>(itemDTO);
                var basePrice = product.PricingItems.First(x => x.ItemId == itemDTO.ItemId).BasePrice / currencyRate;
                var taxAmount = product.ItemTaxes.First(x => x.ItemId == itemDTO.ItemId).Tax.Amount / currencyRate;

                saleInvoiceItem.TotalAmount = InvoiceItemCalculator.CalculateTotalAmount(basePrice, itemDTO.Qty);
                saleInvoiceItem.TotalTaxes = InvoiceItemCalculator.CalculateTotalTaxes(basePrice, taxAmount, itemDTO.Qty);
                saleInvoiceItem.TotalPrice = InvoiceItemCalculator.CalculateTotalPrice(saleInvoiceItem.TotalAmount, saleInvoiceItem.TotalTaxes);
                saleInvoiceItem.BasePrice = basePrice;
                saleInvoiceItem.SaleInvoiceId = invoiceId;

                await repository.AddInvoiceItem(saleInvoiceItem);
            }
        }

        public async Task ReduceStockAsync(CreateSaleInvoiceDTO dto, IUnitOfWork unitOfWork)
        {
            foreach (var item in dto.SaleInvoiceItems)
            {
                if (item.ItemType == ItemType.Product)
                {
                    var stock = await unitOfWork.StockRepository.GetByItemIdAndWarehouseId(item.ItemId, dto.WarehouseId);
                    if (stock == null)
                    {
                        throw new InvalidOperationException($"Stock not found for ItemId {item.ItemId} in the specified warehouse.");
                    }

                    stock.Qty -= item.Qty;
                    unitOfWork.StockRepository.Update(stock);
                }
            }
            await unitOfWork.SaveChangesAsync();
        }
    }
}
