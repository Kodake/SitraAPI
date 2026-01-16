using SITRA.Core.Interfaces;
using Microsoft.AspNetCore.OutputCaching;

namespace SITRA.Core.Mediator.Commands.Products.Products
{
    public class ActivateProductCommand : ToggleCommand
    {
        public ActivateProductCommand(long id) : base(id) { }

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
