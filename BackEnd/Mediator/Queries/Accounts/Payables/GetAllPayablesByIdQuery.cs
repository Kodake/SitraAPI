using AutoMapper;
using BackEnd.DTOs.Accounts.Payables;
using BackEnd.Repositories;

namespace BackEnd.Mediator.Queries.Accounts.Payables
{
    public class GetAllPayablesByIdQuery(long id) : GetByIdQuery<PayableDTO>(id)
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
