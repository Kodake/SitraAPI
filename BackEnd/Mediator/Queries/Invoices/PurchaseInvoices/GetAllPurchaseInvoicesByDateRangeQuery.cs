using AutoMapper;
using BackEnd.DTOs;
using BackEnd.DTOs.Invoices.PurchaseInvoices;
using BackEnd.Repositories;

namespace BackEnd.Mediator.Queries.Invoices.PurchaseInvoices
{
    public class GetAllPurchaseInvoicesByDateRangeQuery : GetAllQuery<PurchaseInvoiceListDTO>
    {
        public DateTime From { get; }
        public DateTime To { get; }

        public GetAllPurchaseInvoicesByDateRangeQuery(DateTime from, DateTime to, PaginationDTO pagination)
            : base(pagination)
        {
            From = from;
            To = to;
        }

        public override async Task<List<PurchaseInvoiceListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var saleInvoices = await unitOfWork.PurchaseInvoiceRepository
                .GetPurchasesByDateRangeAsync(From, To, Pagination);

            return mapper.Map<List<PurchaseInvoiceListDTO>>(saleInvoices);
        }
    }
}
