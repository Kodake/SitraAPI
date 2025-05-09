using AutoMapper;
using BackEnd.DTOs;
using BackEnd.DTOs.Products.Units;
using BackEnd.Repositories;

namespace BackEnd.Mediator.Queries.Products.Units
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
