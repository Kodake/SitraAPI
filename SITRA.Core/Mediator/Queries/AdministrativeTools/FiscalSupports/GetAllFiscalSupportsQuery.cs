using AutoMapper;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.AdministrativeTools.FiscalSupports;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.AdministrativeTools.FiscalSupports
{
    public class GetAllFiscalSupportsQuery : GetAllQuery<FiscalSupportListDTO>
    {
        public GetAllFiscalSupportsQuery(PaginationDTO pagination) : base(pagination) { }

        public override async Task<List<FiscalSupportListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var fiscalSupports = await unitOfWork.FiscalSupportRepository.GetReadOnlyAsync(Pagination);
            return mapper.Map<List<FiscalSupportListDTO>>(fiscalSupports);
        }
    }
}
