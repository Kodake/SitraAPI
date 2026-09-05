using AutoMapper;
using SITRA.Core.DTOs.AdministrativeTools.FiscalSupports;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.AdministrativeTools.FiscalSupports
{
    public class GetDocumentsByTypeQuery(EcfType supportType) : IQuery<FiscalSupportDTO?>
    {
        public EcfType SupportType { get; } = supportType;

        public async Task<FiscalSupportDTO?> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var fiscalSupport = await unitOfWork
                .FiscalSupportRepository
                .GetByDocumentTypeAsync(SupportType, cancellationToken);

            if (fiscalSupport is null)
                return null;

            var dto = mapper.Map<FiscalSupportDTO>(fiscalSupport);
            dto.FiscalSupportNumber = fiscalSupport.GetNextFiscalSupportNumber();

            return dto;
        }
    }
}
