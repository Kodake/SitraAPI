using AutoMapper;
using SITRA.Core.DTOs.Accounts.Receivables;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Receivables
{
    public class GetAllReceivablesByIdQuery : GetByIdQuery<ReceivableDTO>
    {
        public GetAllReceivablesByIdQuery(long id) : base(id) { }

        public override async Task<ReceivableDTO> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var receivable = await unitOfWork.ReceivableRepository.GetAllByIdAsync(Id);
            return mapper.Map<ReceivableDTO>(receivable);
        }
    }
}
