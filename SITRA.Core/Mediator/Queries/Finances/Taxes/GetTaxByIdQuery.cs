using AutoMapper;
using SITRA.Core.DTOs.Finances.Taxes;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Finances.Taxes
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
