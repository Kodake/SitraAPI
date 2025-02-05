using AutoMapper;
using BackEnd.DTOs.Invoices.PurchaseInvoices;
using BackEnd.Repositories;

namespace BackEnd.MediatR.Queries.Invoices.PurchaseInvoices
{
    public class GetPurchaseInvoiceByIdQuery : GetByIdQuery<PurchaseInvoiceDTO>
    {
        public GetPurchaseInvoiceByIdQuery(long id) : base(id) { }

        public override async Task<PurchaseInvoiceDTO> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var saleInvoice = await unitOfWork.PurchaseInvoiceRepository.GetByIdAsync(Id);
            return mapper.Map<PurchaseInvoiceDTO>(saleInvoice);
        }
    }
}
