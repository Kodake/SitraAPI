using AutoMapper;
using BackEnd.DTOs.Products.Models;
using BackEnd.Enums;
using BackEnd.Repositories;

namespace BackEnd.Mediator.Queries.Products.Models
{
    public class GetSelectsModelsQuery : GetSelectsQuery<ModelListDTO>
    {
        public GetSelectsModelsQuery(StatusType status) : base(status) { }

        public override async Task<List<ModelListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var models = await unitOfWork.ModelRepository.GetAll(Status);
            return mapper.Map<List<ModelListDTO>>(models);
        }
    }
}
