using AutoMapper;
using SITRA.Core.DTOs.Products.Models;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Products.Models
{
    public class GetModelByIdQuery : GetByIdQuery<ModelDTO>
    {
        public GetModelByIdQuery(long id) : base(id) { }

        public override async Task<ModelDTO> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var models = await unitOfWork.ModelRepository.GetByIdAsync(Id);
            return mapper.Map<ModelDTO>(models);
        }
    }
}
