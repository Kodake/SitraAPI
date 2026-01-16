using AutoMapper;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Invoices.Quotations;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Invoices.Quotations
{
    public class GetAllQuotationsQuery : GetAllQuery<QuotationListDTO>
    {
        public GetAllQuotationsQuery(PaginationDTO pagination) : base(pagination) { }

        public override async Task<List<QuotationListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var quotations = await unitOfWork.QuotationRepository.GetAllAsync(Pagination);
            return mapper.Map<List<QuotationListDTO>>(quotations);
        }
    }
}
