using AutoMapper;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Invoices.SaleInvoices;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Invoices.SaleInvoices
{
    public class GetAllSaleInvoicesByDateRangeAndBillerQuery : GetAllQuery<SaleInvoiceListDTO>
    {
        public DateTime From { get; }
        public DateTime To { get; }

        public GetAllSaleInvoicesByDateRangeAndBillerQuery(DateTime from, DateTime to, PaginationDTO pagination)
            : base(pagination)
        {
            From = from;
            To = to;
        }

        public override async Task<List<SaleInvoiceListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var saleInvoices = await unitOfWork.SaleInvoiceRepository
                .GetSalesByDateRangeAsync(From, To, Pagination);

            return mapper.Map<List<SaleInvoiceListDTO>>(saleInvoices);
        }
    }
}
