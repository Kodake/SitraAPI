using AutoMapper;
using BackEnd.DTOs;
using BackEnd.DTOs.Invoices.PurchaseInvoices;
using BackEnd.Repositories;

namespace BackEnd.MediatR.Queries.Invoices.PurchaseInvoices
{
    public class GetAllPurchaseInvoicesQuery : GetAllQuery<PurchaseInvoiceListDTO>
    {
        public GetAllPurchaseInvoicesQuery(PaginationDTO pagination) : base(pagination) { }

        public override async Task<List<PurchaseInvoiceListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var saleInvoices = await unitOfWork.PurchaseInvoiceRepository.GetAllAsync(Pagination);
            return mapper.Map<List<PurchaseInvoiceListDTO>>(saleInvoices);
        }
    }
}
