using AutoMapper;
using BackEnd.DTOs;
using BackEnd.DTOs.Warehouses.Transfers;
using BackEnd.Repositories;

namespace BackEnd.MediatR.Queries.Warehouses.Transfers
{
    public class GetAllTransfersQuery : GetAllQuery<TransferListDTO>
    {
        public GetAllTransfersQuery(PaginationDTO pagination) : base(pagination) { }

        public override async Task<List<TransferListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var transfers = await unitOfWork.TransferRepository.GetAllAsync(Pagination);
            return mapper.Map<List<TransferListDTO>>(transfers);
        }
    }
}
