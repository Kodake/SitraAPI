using AutoMapper;
using BackEnd.DTOs.Invoices.SaleInvoices;
using BackEnd.Repositories;

namespace BackEnd.MediatR.Queries.Invoices.SaleInvoices
{
    public class GetSaleInvoiceByIdQuery : GetByIdQuery<SaleInvoiceDTO>
    {
        public GetSaleInvoiceByIdQuery(long id) : base(id) { }

        public override async Task<SaleInvoiceDTO> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var saleInvoice = await unitOfWork.QuotationRepository.GetByIdAsync(Id);
            return mapper.Map<SaleInvoiceDTO>(saleInvoice);
        }
    }
}
