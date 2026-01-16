using AutoMapper;
using SITRA.Core.DTOs.Products.Units;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Products.Units
{
    public class GetUnitByIdQuery : GetByIdQuery<UnitDTO>
    {
        public GetUnitByIdQuery(long id) : base(id) { }

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
