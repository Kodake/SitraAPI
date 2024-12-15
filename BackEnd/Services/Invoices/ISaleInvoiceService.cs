using AutoMapper;
using BackEnd.DTOs.Invoices;
using BackEnd.Entities.Invoices;
using BackEnd.Repositories;
using BackEnd.Repositories.Invoices;

namespace BackEnd.Services.Invoices
{
    public interface ISaleInvoiceService
    {
        Task CalculateInvoiceTotalsAsync(IEnumerable<SaleInvoiceItemDTO> items, SaleInvoice saleInvoice, IUnitOfWork unitOfWork, double currencyRate);
        Task AddInvoiceItemsAsync(IEnumerable<SaleInvoiceItemDTO> items, long invoiceId, ISaleInvoiceRepository repository, IUnitOfWork unitOfWork, IMapper mapper, double currencyRate);
        Task ReduceStockAsync(CreateSaleInvoiceDTO dto, IUnitOfWork unitOfWork);
    }

}
