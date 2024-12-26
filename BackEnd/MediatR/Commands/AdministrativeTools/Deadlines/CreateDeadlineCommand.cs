using AutoMapper;
using BackEnd.DTOs.AdministrativeTools.Deadlines;
using BackEnd.Entities.AdministrativeTools;
using BackEnd.Repositories;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.MediatR.Commands.AdministrativeTools.Deadlines
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
