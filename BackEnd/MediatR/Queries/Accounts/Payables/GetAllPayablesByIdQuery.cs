using AutoMapper;
using BackEnd.DTOs.Accounts.Payables;
using BackEnd.Repositories;

namespace BackEnd.MediatR.Queries.Accounts.Payables
{
    public class GetAllPayablesByIdQuery : GetByIdQuery<PayableDTO>
    {
        public GetAllPayablesByIdQuery(long id) : base(id) { }

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
