using AutoMapper;
using BackEnd.DTOs.AdministrativeTools.Deadlines;
using BackEnd.Repositories;

namespace BackEnd.Mediator.Queries.AdministrativeTools.Deadlines
{
    public class GetDeadlineByIdQuery : GetByIdQuery<DeadlineDTO>
    {
        public GetDeadlineByIdQuery(long id) : base(id) { }

        public override async Task<DeadlineDTO> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var deadline = await unitOfWork.DeadlineRepository.GetByIdAsync(Id);
            return mapper.Map<DeadlineDTO>(deadline);
        }
    }
}
