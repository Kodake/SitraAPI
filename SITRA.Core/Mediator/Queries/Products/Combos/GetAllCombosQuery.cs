using AutoMapper;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Products.Combos;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Products.Combos
{
    public class GetAllCombosQuery : GetAllQuery<ComboListDTO>
    {
        public GetAllCombosQuery(PaginationDTO pagination) : base(pagination) { }

        public override async Task<List<ComboListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var combos = await unitOfWork.ComboRepository.GetReadOnlyAsync(Pagination);
            return mapper.Map<List<ComboListDTO>>(combos);
        }
    }
}
