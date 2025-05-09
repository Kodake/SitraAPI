using AutoMapper;
using BackEnd.DTOs.AdministrativeTools.Deadlines;
using BackEnd.Entities.AdministrativeTools;
using BackEnd.Repositories;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.Mediator.Commands.AdministrativeTools.Deadlines
{
    public class UpdateDeadlineCommand : UpdateCommand<Deadline, CreateDeadlineDTO>
    {
        public UpdateDeadlineCommand(long id, CreateDeadlineDTO dto) : base(id, dto) { }

        public override async Task<bool> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper, CancellationToken cancellationToken)
        {
            var deadlineDB = await unitOfWork.DeadlineRepository.GetByIdAsync(Id);

            if (deadlineDB == null) return false;

            mapper.Map(Dto, deadlineDB);
            unitOfWork.DeadlineRepository.Update(deadlineDB);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("deadlines-get", cancellationToken);

            return true;
        }
    }
}
