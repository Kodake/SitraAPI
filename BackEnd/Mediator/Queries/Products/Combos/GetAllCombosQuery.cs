using AutoMapper;
using BackEnd.DTOs;
using BackEnd.DTOs.Products.Combos;
using BackEnd.Repositories;

namespace BackEnd.Mediator.Queries.Products.Combos
{
    public class GetAllCombosQuery : GetAllQuery<ComboListDTO>
    {
        public GetAllCombosQuery(PaginationDTO pagination) : base(pagination) { }

        public override async Task<List<ComboListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var combos = await unitOfWork.ComboRepository.GetAllAsync(Pagination);
            return mapper.Map<List<ComboListDTO>>(combos);
        }
    }
}
