using AutoMapper;
using BackEnd.DTOs.Invoices.Quotations;
using BackEnd.Repositories;

namespace BackEnd.MediatR.Queries.Invoices.Quotations
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
