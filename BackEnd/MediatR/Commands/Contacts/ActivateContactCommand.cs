using BackEnd.Repositories;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.MediatR.Commands.Contacts
{
    public class ActivateContactCommand : ToggleCommand
    {
        public ActivateContactCommand(int id) : base(id) { }

        public override async Task<bool> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, CancellationToken cancellationToken)
        {
            var contactDB = await unitOfWork.ContactRepository.GetByIdAsync(Id);

            if (contactDB == null) return false;

            await unitOfWork.ContactRepository.ActivateAsync(Id);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("contacts-get", cancellationToken);

            return true;
        }
    }
}
