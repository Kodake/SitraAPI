using AutoMapper;
using BackEnd.DTOs.Products.Models;
using BackEnd.Entities.Products;
using BackEnd.Repositories;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.Mediator.Commands.Products.Models
{
    public class UpdateModelCommand : UpdateCommand<Model, CreateModelDTO>
    {
        public UpdateModelCommand(long id, CreateModelDTO dto) : base(id, dto) { }

        public override async Task<bool> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper, CancellationToken cancellationToken)
        {
            var modelDB = await unitOfWork.ModelRepository.GetByIdAsync(Id);

            if (modelDB == null) return false;

            mapper.Map(Dto, modelDB);
            unitOfWork.ModelRepository.Update(modelDB);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("models-get", default);

            return true;
        }
    }
}
