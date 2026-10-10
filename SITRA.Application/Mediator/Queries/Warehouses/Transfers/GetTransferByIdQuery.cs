using AutoMapper;
using SITRA.Core.DTOs.Warehouses.Transfers;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Warehouses.Transfers
{
    public class GetTransferByIdQuery : GetByIdQuery<TransferDTO>
    {
        public GetTransferByIdQuery(Guid id) : base(id) { }

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
