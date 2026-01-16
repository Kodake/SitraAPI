using AutoMapper;
using SITRA.Core.DTOs.AdministrativeTools.FiscalSupports;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Products.Items
{
    public class GetDocumentsByTypeIdAndWarehouseIdQuery(FiscalSupportType supportType, long warehouseId) : IQuery<FiscalSupportDTO>
    {
        public FiscalSupportType SupportType { get; } = supportType;
        public long WarehouseId { get; } = warehouseId;

        public async Task<FiscalSupportDTO> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var documents = await unitOfWork.FiscalSupportRepository.GetByDocumentTypeAndWarehouseIdAsync(SupportType, WarehouseId);
            var documentsDTO = mapper.Map<FiscalSupportDTO>(documents);
            return documentsDTO;
        }
    }
}
