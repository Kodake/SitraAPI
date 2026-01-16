using AutoMapper;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Products.Combos
{
    public class GetComboByIdQuery : GetByIdQuery<ComboDTO>
    {
        public GetComboByIdQuery(long id) : base(id) { }

        public override async Task<ComboDTO> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var combo = await unitOfWork.ComboRepository.GetByIdAsync(Id);
            return mapper.Map<ComboDTO>(combo);
        }
    }
}
