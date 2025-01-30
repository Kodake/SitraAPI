using AutoMapper;
using BackEnd.DTOs;
using BackEnd.DTOs.Invoices.Quotations;
using BackEnd.Repositories;

namespace BackEnd.MediatR.Queries.Invoices.Quotations
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
