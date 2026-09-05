using AutoMapper;
using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.DTOs.Products.Models;
using SITRA.Core.Entities.Products;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Commands.Products.Models
{
    public class UpdateModelCommand : UpdateCommand<Model, CreateModelDTO>
    {
        public UpdateModelCommand(Guid id, CreateModelDTO dto) : base(id, dto) { }

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
