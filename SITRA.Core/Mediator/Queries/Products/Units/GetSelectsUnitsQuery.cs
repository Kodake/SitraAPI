using AutoMapper;
using SITRA.Core.DTOs.Products.Units;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Products.Units
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
