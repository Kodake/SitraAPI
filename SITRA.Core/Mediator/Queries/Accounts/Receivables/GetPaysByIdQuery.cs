using AutoMapper;
using SITRA.Core.DTOs.Accounts.Receivables;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Receivables
{
    public class GetPaysByIdQuery : GetByIdQuery<ReceivableDTO>
    {
        public GetPaysByIdQuery(Guid id) : base(id) { }

        public override async Task<ReceivableDTO> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var receivable = await unitOfWork.ReceivableRepository.GetPaysByIdAsync(Id);
            return mapper.Map<ReceivableDTO>(receivable);
        }
    }
}
