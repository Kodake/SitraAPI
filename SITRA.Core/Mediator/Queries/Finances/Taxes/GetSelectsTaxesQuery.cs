using AutoMapper;
using SITRA.Core.DTOs.Finances.Taxes;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Finances.Taxes
{
    public class GetSelectsTaxesQuery : GetSelectsQuery<TaxListDTO>
    {
        public GetSelectsTaxesQuery(StatusType status) : base(status) { }

        public override async Task<List<TaxListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var taxes = await unitOfWork.TaxRepository.GetAll(Status);
            return mapper.Map<List<TaxListDTO>>(taxes);
        }
    }
}
