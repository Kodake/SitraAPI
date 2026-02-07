using AutoMapper;
using SITRA.Core.DTOs.AdministrativeTools.FiscalSupports;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Products.Items
{
    public class GetDocumentsByTypeIdAndWarehouseIdQuery(FiscalSupportType supportType, Guid warehouseId) : IQuery<FiscalSupportDTO?>
    {
        public FiscalSupportType SupportType { get; } = supportType;
        public Guid WarehouseId { get; } = warehouseId;

        public async Task<FiscalSupportDTO?> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var fiscalSupport = await unitOfWork
                .FiscalSupportRepository
                .GetByDocumentTypeAndWarehouseIdAsync(SupportType, WarehouseId);

            if (fiscalSupport is null)
                return null;

            var dto = mapper.Map<FiscalSupportDTO>(fiscalSupport);
            dto.FiscalSupportNumber = fiscalSupport.GetNextFiscalSupportNumber();

            return dto;
        }
    }
}
