using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Commands.Contacts
{
    public class DeactivateContactCommand : ToggleCommand
    {
        public DeactivateContactCommand(Guid id) : base(id) { }

        public override async Task<bool> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, CancellationToken cancellationToken)
        {
            var contactDB = await unitOfWork.ContactRepository.GetByIdAsync(Id);

            if (contactDB == null) return false;

            await unitOfWork.ContactRepository.DeactivateAsync(Id);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("contacts-get", cancellationToken);

            return true;
        }
    }
}
