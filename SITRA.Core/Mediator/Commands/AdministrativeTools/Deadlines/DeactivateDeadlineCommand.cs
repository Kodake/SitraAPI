using SITRA.Core.Interfaces;
using Microsoft.AspNetCore.OutputCaching;

namespace SITRA.Core.Mediator.Commands.AdministrativeTools.Deadlines
{
    public class DeactivateDeadlineCommand : ToggleCommand
    {
        public DeactivateDeadlineCommand(Guid id) : base(id) { }

        public override async Task<bool> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, CancellationToken cancellationToken)
        {
            var deadlineDB = await unitOfWork.DeadlineRepository.GetByIdAsync(Id);

            if (deadlineDB == null) return false;

            await unitOfWork.DeadlineRepository.DeactivateAsync(Id);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("deadlines-get", cancellationToken);

            return true;
        }
    }
}
