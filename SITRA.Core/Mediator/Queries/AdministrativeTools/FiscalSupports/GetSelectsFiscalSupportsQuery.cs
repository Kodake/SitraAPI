using AutoMapper;
using SITRA.Core.DTOs.AdministrativeTools.FiscalSupports;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.AdministrativeTools.FiscalSupports
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
