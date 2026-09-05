using AutoMapper;
using SITRA.Core.DTOs.Accounts.Payables;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Accounts.Payables
{
    public class GetAllPayablesByIdQuery(Guid id) : GetByIdQuery<PayableDTO>(id)
    {
        public override async Task<PayableDTO> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var payable = await unitOfWork.PayableRepository.GetAllByIdAsync(Id);
            return mapper.Map<PayableDTO>(payable);
        }
    }
}
