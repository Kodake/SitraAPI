using AutoMapper;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Invoices.SaleInvoices;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Invoices.SaleInvoices
{
    public class GetAllSaleInvoicesQuery : GetAllQuery<SaleInvoiceListDTO>
    {
        public GetAllSaleInvoicesQuery(PaginationDTO pagination) : base(pagination) { }

        public override async Task<List<SaleInvoiceListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var saleInvoices = await unitOfWork.SaleInvoiceRepository.GetReadOnlyAsync(Pagination);
            return mapper.Map<List<SaleInvoiceListDTO>>(saleInvoices);
        }
    }
}
