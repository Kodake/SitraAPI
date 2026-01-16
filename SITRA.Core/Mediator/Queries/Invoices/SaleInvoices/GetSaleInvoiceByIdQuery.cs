using AutoMapper;
using SITRA.Core.DTOs.Invoices.SaleInvoices;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Invoices.SaleInvoices
{
    public class GetSaleInvoiceByIdQuery(long id) : GetByIdQuery<SaleInvoiceDTO>(id)
    {
        public override async Task<SaleInvoiceDTO> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var saleInvoice = await unitOfWork.SaleInvoiceRepository.GetByIdAsync(Id);
            return mapper.Map<SaleInvoiceDTO>(saleInvoice);
        }
    }
}
