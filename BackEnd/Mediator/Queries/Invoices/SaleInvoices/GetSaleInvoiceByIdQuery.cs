using AutoMapper;
using BackEnd.DTOs.Invoices.SaleInvoices;
using BackEnd.Repositories;

namespace BackEnd.Mediator.Queries.Invoices.SaleInvoices
{
    public class GetSaleInvoiceByIdQuery : GetByIdQuery<SaleInvoiceDTO>
    {
        public GetSaleInvoiceByIdQuery(long id) : base(id) { }

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
