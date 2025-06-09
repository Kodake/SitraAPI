using AutoMapper;
using BackEnd.DTOs.Products.Models;
using BackEnd.Repositories;

namespace BackEnd.Mediator.Queries.Products.Models
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
