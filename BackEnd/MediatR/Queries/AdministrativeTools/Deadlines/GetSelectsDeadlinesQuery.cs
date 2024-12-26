using AutoMapper;
using BackEnd.DTOs.AdministrativeTools.Deadlines;
using BackEnd.Enums;
using BackEnd.Repositories;

namespace BackEnd.MediatR.Queries.AdministrativeTools.Deadlines
{
    public class GetSelectsDeadlinesQuery : GetSelectsQuery<DeadlineListDTO>
    {
        public GetSelectsDeadlinesQuery(StatusType status = StatusType.All) : base(status) { }

        public override async Task<List<DeadlineListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var deadlines = await unitOfWork.DeadlineRepository.GetAll(Status);
            return mapper.Map<List<DeadlineListDTO>>(deadlines);
        }
    }
}
