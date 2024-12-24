using BackEnd.Repositories;
using AutoMapper;
using BackEnd.Enums;
using BackEnd.DTOs.Products.Units;

namespace BackEnd.MediatR.Queries.Products.Units
{
    public class GetSelectsUnitsQuery : GetSelectsQuery<UnitListDTO>
    {
        public GetSelectsUnitsQuery(StatusType status) : base(status) { }

        public override async Task<List<UnitListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var units = await unitOfWork.UnitRepository.GetAll(Status);
            return mapper.Map<List<UnitListDTO>>(units);
        }
    }
}
