using AutoMapper;
using SITRA.Core.DTOs.Accounts.Payables;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Accounts.Payables
{
    public class GetPaysByIdQuery : GetByIdQuery<PayableDTO>
    {
        public GetPaysByIdQuery(long id) : base(id) { }

        public override async Task<PayableDTO> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var payable = await unitOfWork.PayableRepository.GetPaysByIdAsync(Id);
            return mapper.Map<PayableDTO>(payable);
        }
    }
}
