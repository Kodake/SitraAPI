using BackEnd.Repositories;
using AutoMapper;
using BackEnd.DTOs.Products.Units;

namespace BackEnd.MediatR.Queries.Products.Units
{
    public class GetUnitByIdQuery : GetByIdQuery<UnitDTO>
    {
        public GetUnitByIdQuery(int id) : base(id) { }

        public override async Task<UnitDTO> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var unit = await unitOfWork.UnitRepository.GetByIdAsync(Id);
            return mapper.Map<UnitDTO>(unit);
        }
    }
}