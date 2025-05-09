using AutoMapper;
using BackEnd.DTOs;
using BackEnd.DTOs.Invoices.SaleInvoices;
using BackEnd.Repositories;

namespace BackEnd.Mediator.Queries.Invoices.SaleInvoices
{
    public class GetAllSaleInvoicesQuery : GetAllQuery<SaleInvoiceListDTO>
    {
        public GetAllSaleInvoicesQuery(PaginationDTO pagination) : base(pagination) { }

        public override async Task<List<SaleInvoiceListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var saleInvoices = await unitOfWork.SaleInvoiceRepository.GetAllAsync(Pagination);
            return mapper.Map<List<SaleInvoiceListDTO>>(saleInvoices);
        }
    }
}
