using AutoMapper;
using BackEnd.DTOs.Finances.Taxes;
using BackEnd.Enums;
using BackEnd.Repositories;

namespace BackEnd.MediatR.Queries.Finances.Taxes
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
