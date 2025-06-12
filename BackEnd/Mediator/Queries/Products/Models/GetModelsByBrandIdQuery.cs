using AutoMapper;
using BackEnd.DTOs.Products.Models;
using BackEnd.Repositories;

namespace BackEnd.Mediator.Queries.Products.Models
{
    public class GetModelsByBrandIdQuery(long brandId) : IQuery<List<ModelListDTO>>
    {
        public long BrandId { get; } = brandId;

        public async Task<List<ModelListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var models = await unitOfWork.ModelRepository.GetByBrandId(BrandId);
            var modelsDTO = mapper.Map<List<ModelListDTO>>(models);
            return modelsDTO;
        }
    }
}
