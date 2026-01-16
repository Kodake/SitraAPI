using AutoMapper;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Invoices.PurchaseInvoices;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Invoices.PurchaseInvoices
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
