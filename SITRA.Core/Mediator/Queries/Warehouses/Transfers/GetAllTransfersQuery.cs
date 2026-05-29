using AutoMapper;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Warehouses.Transfers;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Warehouses.Transfers
{
    public class GetAllTransfersQuery : GetAllQuery<TransferListDTO>
    {
        public GetAllTransfersQuery(PaginationDTO pagination) : base(pagination) { }

        public override async Task<List<TransferListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var transfers = await unitOfWork.TransferRepository.GetReadOnlyAsync(Pagination);
            return mapper.Map<List<TransferListDTO>>(transfers);
        }
    }
}
