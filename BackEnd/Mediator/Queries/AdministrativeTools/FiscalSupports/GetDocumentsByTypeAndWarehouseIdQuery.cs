using AutoMapper;
using BackEnd.DTOs.AdministrativeTools.FiscalSupports;
using BackEnd.Enums;
using BackEnd.Repositories;

namespace BackEnd.Mediator.Queries.Products.Items
{
    public class GetDocumentsByTypeIdAndWarehouseIdQuery(FiscalSupportType supportType, long warehouseId) : IQuery<FiscalSupportDTO>
    {
        public FiscalSupportType SupportType { get; } = supportType;
        public long WarehouseId { get; } = warehouseId;

        public async Task<FiscalSupportDTO?> GetData(
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
