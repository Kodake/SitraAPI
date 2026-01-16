using AutoMapper;
using SITRA.Core.DTOs.Products.Models;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Products.Models
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
