using AutoMapper;
using BackEnd.DTOs.Warehouses.Transfers;
using BackEnd.Repositories;

namespace BackEnd.MediatR.Queries.Warehouses.Transfers
{
    public class GetTransferByIdQuery : GetByIdQuery<TransferDTO>
    {
        public GetTransferByIdQuery(int id) : base(id) { }

        public override async Task<TransferDTO> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var transfer = await unitOfWork.TransferRepository.GetByIdAsync(Id);
            return mapper.Map<TransferDTO>(transfer);
        }
    }
}
