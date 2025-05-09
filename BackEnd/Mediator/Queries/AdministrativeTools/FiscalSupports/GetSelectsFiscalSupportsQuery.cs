using AutoMapper;
using BackEnd.DTOs.AdministrativeTools.FiscalSupports;
using BackEnd.Enums;
using BackEnd.Repositories;

namespace BackEnd.Mediator.Queries.AdministrativeTools.FiscalSupports
{
    public class GetSelectsFiscalSupportsQuery : GetSelectsQuery<FiscalSupportListDTO>
    {
        public GetSelectsFiscalSupportsQuery(StatusType status) : base(status) { }

        public override async Task<List<FiscalSupportListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var fiscalSupports = await unitOfWork.FiscalSupportRepository.GetAll(Status);
            return mapper.Map<List<FiscalSupportListDTO>>(fiscalSupports);
        }
    }
}
