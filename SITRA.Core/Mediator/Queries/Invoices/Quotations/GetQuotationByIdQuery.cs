using AutoMapper;
using SITRA.Core.DTOs.Invoices.Quotations;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Invoices.Quotations
{
    public class GetQuotationByIdQuery : GetByIdQuery<QuotationDTO>
    {
        public GetQuotationByIdQuery(long id) : base(id) { }

        public override async Task<QuotationDTO> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var quotation = await unitOfWork.QuotationRepository.GetByIdAsync(Id);
            return mapper.Map<QuotationDTO>(quotation);
        }
    }
}
