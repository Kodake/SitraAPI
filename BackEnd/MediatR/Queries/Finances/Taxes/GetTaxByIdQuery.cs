using AutoMapper;
using BackEnd.DTOs.Finances.Taxes;
using BackEnd.Repositories;

namespace BackEnd.MediatR.Queries.Finances.Taxes
{
    public class GetTaxByIdQuery : GetByIdQuery<TaxDTO>
    {
        public GetTaxByIdQuery(long id) : base(id) { }

        public override async Task<TaxDTO> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var tax = await unitOfWork.TaxRepository.GetByIdAsync(Id);
            return mapper.Map<TaxDTO>(tax);
        }
    }
}
