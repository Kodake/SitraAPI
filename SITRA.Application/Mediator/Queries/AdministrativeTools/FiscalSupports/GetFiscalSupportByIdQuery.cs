using AutoMapper;
using SITRA.Core.DTOs.AdministrativeTools.FiscalSupports;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.AdministrativeTools.FiscalSupports
{
    public class GetFiscalSupportByIdQuery : GetByIdQuery<FiscalSupportDTO>
    {
        public GetFiscalSupportByIdQuery(Guid id) : base(id) { }

        public override async Task<FiscalSupportDTO> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var fiscalSupport = await unitOfWork.FiscalSupportRepository.GetByIdAsync(Id);
            return mapper.Map<FiscalSupportDTO>(fiscalSupport);
        }
    }
}
