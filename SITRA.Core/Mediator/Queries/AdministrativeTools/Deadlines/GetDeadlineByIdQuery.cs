using AutoMapper;
using SITRA.Core.DTOs.AdministrativeTools.Deadlines;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.AdministrativeTools.Deadlines
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
