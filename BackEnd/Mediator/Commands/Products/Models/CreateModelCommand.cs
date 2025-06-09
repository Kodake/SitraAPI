using AutoMapper;
using BackEnd.DTOs.Products.Models;
using BackEnd.Entities.Products;
using BackEnd.Repositories;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.Mediator.Commands.Products.Models
{
    public class CreateModelCommand : CreateCommand<Model, CreateModelDTO>
    {
        public CreateModelCommand(CreateModelDTO dto) : base(dto) { }

        public override async Task<CreateModelDTO> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper, CancellationToken cancellationToken)
        {
            var model = mapper.Map<Model>(Dto);
            await unitOfWork.ModelRepository.CreateAsync(model);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("models-get", cancellationToken);
            return Dto;
        }
    }
}
