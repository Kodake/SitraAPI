using BackEnd.Repositories;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.MediatR.Commands.Products.Products
{
    public class ActivateProductCommand : ToggleCommand
    {
        public ActivateProductCommand(int id) : base(id) { }

        public override async Task<bool> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, CancellationToken cancellationToken)
        {
            var productDB = await unitOfWork.ProductRepository.GetByIdAsync(Id);

            if (productDB == null) return false;

            await unitOfWork.ProductRepository.ActivateAsync(Id);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("products-get", cancellationToken);

            return true;
        }
    }
}