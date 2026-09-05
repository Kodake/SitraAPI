using AutoMapper;
using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.DTOs.AdministrativeTools.Deadlines;
using SITRA.Core.Entities.AdministrativeTools;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Commands.AdministrativeTools.Deadlines
{
    public class CreateDeadlineCommand : CreateCommand<Deadline, CreateDeadlineDTO>
    {
        public CreateDeadlineCommand(CreateDeadlineDTO dto) : base(dto) { }

        public override async Task<CreateDeadlineDTO> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper, CancellationToken cancellationToken)
        {
            var deadline = mapper.Map<Deadline>(Dto);
            await unitOfWork.DeadlineRepository.CreateAsync(deadline);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("deadlines-get", cancellationToken);
            return Dto;
        }
    }
}
