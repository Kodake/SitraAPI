using AutoMapper;
using BackEnd.DTOs.AdministrativeTools.FiscalSupports;
using BackEnd.Enums;
using BackEnd.Repositories;

namespace BackEnd.MediatR.Queries.Products.Items
{
    public class GetDocumentsByTypeIdAndWarehouseIdQuery(FiscalSupportType supportType, long warehouseId) : IQuery<List<FiscalSupportDTO>>
    {
        public FiscalSupportType SupportType { get; } = supportType;
        public long WarehouseId { get; } = warehouseId;

        public async Task<List<FiscalSupportDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var documents = await unitOfWork.FiscalSupportRepository.GetByDocumentTypeAndWarehouseId(SupportType, WarehouseId);
            var documentsDTO = mapper.Map<List<FiscalSupportDTO>>(documents);
            return documentsDTO;
        }
    }
}
