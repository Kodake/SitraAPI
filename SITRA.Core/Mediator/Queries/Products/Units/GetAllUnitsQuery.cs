using AutoMapper;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Products.Units;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Products.Units
{
    public class GetAllUnitsQuery : GetAllQuery<UnitListDTO>
    {
        public GetAllUnitsQuery(PaginationDTO pagination) : base(pagination) { }

        public override async Task<List<UnitListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var units = await unitOfWork.UnitRepository.GetAllAsync(Pagination);
            return mapper.Map<List<UnitListDTO>>(units);
        }
    }
}
