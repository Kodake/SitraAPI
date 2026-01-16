using AutoMapper;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Products.Models;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Products.Models
{
    public class GetAllModelsQuery : GetAllQuery<ModelListDTO>
    {
        public GetAllModelsQuery(PaginationDTO pagination) : base(pagination) { }

        public override async Task<List<ModelListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var models = await unitOfWork.ModelRepository.GetAllAsync(Pagination);
            return mapper.Map<List<ModelListDTO>>(models);
        }
    }
}
