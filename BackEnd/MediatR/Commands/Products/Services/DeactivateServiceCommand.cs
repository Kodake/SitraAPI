using BackEnd.Repositories;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.MediatR.Commands.Products.Services
{
    public class DeactivateServiceCommand : ToggleCommand
    {
        public DeactivateServiceCommand(int id) : base(id) { }

        public override async Task<bool> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, CancellationToken cancellationToken)
        {
            var serviceDB = await unitOfWork.ServiceRepository.GetByIdAsync(Id);

            if (serviceDB == null) return false;

            await unitOfWork.ServiceRepository.DeactivateAsync(Id);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("services-get", cancellationToken);

            return true;
        }
    }
}
