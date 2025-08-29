using AutoMapper;
using BackEnd.Repositories;

namespace BackEnd.Mediator.Queries.Products.Combos
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