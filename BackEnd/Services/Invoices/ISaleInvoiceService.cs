using AutoMapper;
using BackEnd.DTOs.Invoices;
using BackEnd.Entities.Invoices;
using BackEnd.Repositories.Invoices;
using BackEnd.Repositories.Products;
using BackEnd.Repositories.Warehouses;

namespace BackEnd.Services.Invoices
{
    public interface ISaleInvoiceService
    {
        Task CalculateInvoiceTotalsAsync(IEnumerable<SaleInvoiceItemDTO> items, SaleInvoice saleInvoice, IProductRepository productRepository, double currencyRate);
        Task AddInvoiceItemsAsync(IEnumerable<SaleInvoiceItemDTO> items, long invoiceId, ISaleInvoiceRepository repository, IProductRepository productRepository, IMapper mapper, double currencyRate);
        Task ReduceStockAsync(CreateSaleInvoiceDTO dto, IStockRepository stockRepository);
    }

}
