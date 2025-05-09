using BackEnd.Repositories;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.Mediator.Commands.AdministrativeTools.Deadlines
{
    public class DeactivateDeadlineCommand : ToggleCommand
    {
        public DeactivateDeadlineCommand(long id) : base(id) { }

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
