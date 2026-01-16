using AutoMapper;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.AdministrativeTools.Deadlines;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.AdministrativeTools.Deadlines
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
