using AutoMapper;
using BackEnd.DTOs;
using BackEnd.DTOs.AdministrativeTools.Deadlines;
using BackEnd.Repositories;

namespace BackEnd.Mediator.Queries.AdministrativeTools.Deadlines
{
    public class GetAllDeadlinesQuery : GetAllQuery<DeadlineListDTO>
    {
        public GetAllDeadlinesQuery(PaginationDTO pagination) : base(pagination) { }

        public override async Task<List<DeadlineListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var deadlines = await unitOfWork.DeadlineRepository.GetAllAsync(Pagination);
            return mapper.Map<List<DeadlineListDTO>>(deadlines);
        }
    }
}
